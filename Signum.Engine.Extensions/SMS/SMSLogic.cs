﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Engine.Maps;
using Signum.Engine.DynamicQuery;
using System.Reflection;
using Signum.Entities.SMS;
using Signum.Engine.Extensions.Properties;
using Signum.Entities;
using Signum.Utilities;
using Signum.Utilities.Reflection;
using Signum.Engine.Operations;
using Signum.Engine.Processes;
using Signum.Entities.Processes;
using Signum.Engine.Extensions.SMS;
using System.Linq.Expressions;
using Signum.Entities.Basics;
using System.Text.RegularExpressions;

namespace Signum.Engine.SMS
{
    public static class SMSLogic
    {
        static Func<SMSMessageDN, string> SMSSendAndGetTicketAction;
        static Func<CreateMessageParams, List<string>, List<string>> SMSMultipleSendAction;
        static Func<SMSMessageDN, SendState> SMSUpdateStatusAction;

        public static void AssertStarted(SchemaBuilder sb)
        {
            sb.AssertDefined(ReflectionTools.GetMethodInfo(() => Start(null, null, false)));
        }

        public static void Start(SchemaBuilder sb, DynamicQueryManager dqm, bool registerGraph)
        {
            if (sb.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                sb.Include<SMSMessageDN>();

                dqm[typeof(SMSMessageDN)] = (from m in Database.Query<SMSMessageDN>()
                                             select new
                                             {
                                                 Entity = m.ToLite(),
                                                 m.Id,
                                                 Source = m.From,
                                                 m.DestinationNumber,
                                                 m.State,
                                                 m.SendDate,
                                                 m.Template
                                             }).ToDynamic();

                dqm[typeof(SMSTemplateDN)] = (from t in Database.Query<SMSTemplateDN>()
                                              select new
                                              {
                                                  Entity = t.ToLite(),
                                                  t.Id,
                                                  t.Name,
                                                  IsActive = t.IsActiveNow(),
                                                  Message = t.Message.Etc(50),
                                                  Source = t.From,
                                                  AssociatedType = t.AssociatedType.ToLite(),
                                                  t.State,
                                                  t.StartDate,
                                                  t.EndDate,
                                              }).ToDynamic();

                if (registerGraph)
                {
                    SMSMessageGraph.Register();
                    SMSTemplateGraph.Register();
                }
            }
        }

        static Dictionary<Type, LambdaExpression> phoneNumberProviders = new Dictionary<Type, LambdaExpression>();

        public static void RegisterPhoneNumberProvider<T>(Expression<Func<T, string>> func) where T : IdentifiableEntity
        {
            phoneNumberProviders[typeof(T)] = func;

            new BasicConstructFromMany<T, ProcessExecutionDN>(SMSProviderOperations.SendSMSMessage)
            {
                Construct = (providers, args) =>
                {
                    var numbers = Database.Query<T>().Where(p => providers.Contains(p.ToLite()))
                        .Select(func).AsEnumerable().NotNull().Distinct().ToList();

                    CreateMessageParams createParams = args.GetArg<CreateMessageParams>(0);

                    if (!createParams.Message.HasText())
                        throw new ApplicationException("The text for the SMS message has not been set");

                    SMSPackageDN package = new SMSPackageDN
                    {
                        NumLines = numbers.Count,
                    }.Save();

                    var packLite = package.ToLite();

                    numbers.Select(n => createParams.CreateStaticSMSMessage(n, packLite)).SaveList();

                    var process = ProcessLogic.Create(SMSMessageProcess.Send, package);

                    process.ToLite().ExecuteLite(ProcessOperation.Execute);

                    return process;
                }
            }.Register();
        }

        [Serializable]
        public class CreateMessageParams
        {
            public string Message;
            public string From;

            public SMSMessageDN CreateStaticSMSMessage(string destinationNumber, Lite<SMSPackageDN> packLite)
            {
                return new SMSMessageDN
                {
                    Message = this.Message,
                    From = this.From,
                    State = SMSMessageState.Created,
                    DestinationNumber = destinationNumber,
                    SendPackage = packLite
                };
            }
        }

        public static string GetPhoneNumber<T>(T entity) where T : IIdentifiable
        {
            return ((Expression<Func<T, string>>)phoneNumberProviders[typeof(T)]).Invoke(entity);
        }

        #region Message composition

        static Dictionary<Type, LambdaExpression> dataObjectProviders = new Dictionary<Type, LambdaExpression>();

        public static List<Lite<TypeDN>> RegisteredDataObjectProviders()
        {
            return dataObjectProviders.Keys.Select(t => TypeLogic.ToTypeDN(t).ToLite()).ToList();
        }

        public static List<string> GetLiteralsFromDataObjectProvider(Type type)
        {
            if (!dataObjectProviders.ContainsKey(type))
                throw new ArgumentOutOfRangeException("The type {0} is not a registered data provider"
                    .Formato(type.FullName));

            return dataObjectProviders[type].GetType().GetGenericArguments()[0]
                .GetGenericArguments()[1].GetProperties().Select(p => "{{{0}}}".Formato(p.Name)).ToList();
        }

        public static void RegisterDataObjectProvider<T, A>(Expression<Func<T, A>> func) where T : IdentifiableEntity
        {
            dataObjectProviders[typeof(T)] = func;

            new BasicConstructFromMany<T, ProcessExecutionDN>(SMSProviderOperations.SendSMSMessagesFromTemplate)
            {
                Construct = (providers, args) =>
                {
                    var template = args.GetArg<SMSTemplateDN>(0);

                    if (TypeLogic.DnToType[template.AssociatedType] != typeof(T))
                        throw new ArgumentException("The SMS template is associated with the type {0} instead of {1}"
                            .Formato(template.AssociatedType.FullClassName, typeof(T).FullName));

                    var phoneFunc = (Expression<Func<T, string>>)phoneNumberProviders.
                        GetOrThrow(typeof(T), "{0} is not registered as PhoneNumberProvider");

                    var numbers = Database.Query<T>().Where(p => providers.Contains(p.ToLite()))
                          .Select(p => new
                          {
                              Phone = phoneFunc.Invoke(p),
                              Data = func.Invoke(p)
                          }).Where(n => !n.Phone.HasText()).AsEnumerable().ToList();

                    SMSPackageDN package = new SMSPackageDN { NumLines = numbers.Count, }.Save();
                    var packLite = package.ToLite();

                    numbers.Select(n => new SMSMessageDN
                    {
                        Message = template.ComposeMessage(n.Data),
                        From = template.From,
                        DestinationNumber = n.Phone,
                        SendPackage = packLite,
                        State = SMSMessageState.Created,
                    }).SaveList();

                    var process = ProcessLogic.Create(SMSMessageProcess.Send, package);

                    process.ToLite().ExecuteLite(ProcessOperation.Execute);

                    return process;
                }
            }.Register();

            new BasicConstructFrom<T, SMSMessageDN>(SMSMessageOperations.CreateSMSMessageFromTemplate)
            {
                Construct = (provider, args) =>
                {
                    var template = args.GetArg<SMSTemplateDN>(0);

                    if (TypeLogic.DnToType[template.AssociatedType] != typeof(T))
                        throw new ArgumentException("The SMS template is associated with the type {0} instead of {1}"
                            .Formato(template.AssociatedType.FullClassName, typeof(T).FullName));

                    var phoneFunc = (Expression<Func<T, string>>)phoneNumberProviders.
                        GetOrThrow(typeof(T), "{0} is not registered as PhoneNumberProvider");

                    template.MessageLengthExceeded = MessageLengthExceeded.Allowed;

                    return new SMSMessageDN
                    {
                        Message = template.ComposeMessage(func.Invoke(provider)),
                        From = template.From,
                        DestinationNumber = GetPhoneNumber(provider),
                        State = SMSMessageState.Created,
                    };
                }
            }.Register();
        }

        static string literalDelimiterStart = "{";
        public static string LiteralDelimiterStart
        {
            get { return literalDelimiterStart; }
        }

        static string literalDelimiterEnd = "}";
        public static string LiteralDelimiterEnd
        {
            get { return literalDelimiterEnd; }
        }


        static Regex literalFinder = new Regex(@"{(?<name>[_\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nl}][_\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nl}\p{Nd}]*)}");

        static string ComposeMessage(this SMSTemplateDN template, object o)
        {
            if (o == null)
                return template.Message;

            var matches = literalFinder.Matches(template.Message);

            if (matches.Count == 0)
                return template.Message;

            Type t = o.GetType();

            var combinations = (from Match m in literalFinder.Matches(template.Message)
                                select new Combination
                                {
                                    Name = m.Groups["name"].Value,
                                    Value = t.GetProperty(m.Groups["name"].Value).TryCC(fi => fi.GetValue(o, null)).TryToString()
                                }).ToList();

            return CombineText(template, combinations);
        }

        internal class Combination
        {
            public string Name;
            public string Value;
        }

        static string CombineText(SMSTemplateDN template, List<Combination> combinations)
        {
            string text = template.Message;
            if (template.RemoveNoSMSCharacters)
            {
                text = SMSCharacters.RemoveNoSMSCharacters(template.Message);
                combinations.ForEach(c => c.Value = SMSCharacters.RemoveNoSMSCharacters(c.Value));
            }
            return CombineText(text, combinations, template.MessageLengthExceeded);

        }

        static string CombineText(string text, List<Combination> combinations, MessageLengthExceeded onExceeded)
        {
            string result = literalFinder.Replace(text, m => combinations.Single(c => c.Name == m.Groups["name"].Value).Value);
            int remainingLength = SMSCharacters.RemainingLength(result);
            if (remainingLength < 0)
            {
                switch (onExceeded)
                {
                    case MessageLengthExceeded.NotAllowed:
                        throw new ApplicationException("The text for the SMS message exceeds the limit");
                    case MessageLengthExceeded.Allowed:
                        break;
                    case MessageLengthExceeded.TextPruning:
                        return result.RemoveRight(Math.Abs(remainingLength));
                }
            }

            return result;
        }


        internal class CombinedLiteral
        {
            public string Name;
            public string Value;
        }


        #endregion



        #region processes

        public static void StartProcesses(SchemaBuilder sb, DynamicQueryManager dqm)
        {
            if (sb.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                if (!SMSMessageGraph.Registered)
                    throw new InvalidOperationException("SMSMessageGraph must be registered prior to start the processes");

                if (!SMSTemplateGraph.Registered)
                    throw new InvalidOperationException("SMSTemplateGraph must be registered prior to start the processes");

                sb.Include<SMSPackageDN>();
                SMSLogic.AssertStarted(sb);
                ProcessLogic.AssertStarted(sb);
                ProcessLogic.Register(SMSMessageProcess.Send, new SMSMessageSendProcessAlgortihm());
                ProcessLogic.Register(SMSMessageProcess.UpdateStatus, new SMSMessageUpdateStatusProcessAlgorithm());

                new BasicConstructFromMany<SMSMessageDN, ProcessExecutionDN>(SMSMessageOperations.CreateUpdateStatusPackage)
                {
                    Construct = (messages, _) => UpdateMessages(messages.RetrieveFromListOfLite())
                }.Register();

                dqm[typeof(SMSPackageDN)] = (from e in Database.Query<SMSPackageDN>()
                                             select new
                                             {
                                                 Entity = e.ToLite(),
                                                 e.Id,
                                                 e.Name,
                                                 e.NumLines,
                                                 e.NumErrors,
                                             }).ToDynamic();
            }
        }

        private static ProcessExecutionDN UpdateMessages(List<SMSMessageDN> messages)
        {
            SMSPackageDN package = new SMSPackageDN
            {
                NumLines = messages.Count,
            }.Save();

            var packLite = package.ToLite();

            if (messages.Any(m => m.State != SMSMessageState.Sent))
                throw new ApplicationException("SMS messages must be sent prior to update the status");

            messages.Select(m => m.Do(ms => ms.SendPackage = packLite)).SaveList();

            var process = ProcessLogic.Create(SMSMessageProcess.Send, package);

            process.ToLite().ExecuteLite(ProcessOperation.Execute);

            return process;
        }



        #endregion

        public static void RegisterSMSSendAction(Func<SMSMessageDN, string> action)
        {
            SMSSendAndGetTicketAction = action;
        }

        public static void RegisterSMSMultipleSendAction(Func<CreateMessageParams, List<string>, List<string>> action)
        {
            SMSMultipleSendAction = action;
        }

        public static void RegisterSMSUpdateStatusAction(Func<SMSMessageDN, SendState> action)
        {
            SMSUpdateStatusAction = action;
        }

        public static void SendSMS(SMSMessageDN message)
        {
            if (SMSSendAndGetTicketAction == null)
                throw new InvalidOperationException("SMSSendAction was not established");
            SendSMS(message, SMSSendAndGetTicketAction);
        }

        //Allows concurrent custom sendProviders for one application
        public static void SendSMS(SMSMessageDN message, Func<SMSMessageDN, string> sendAndGetTicket)
        {
            message.MessageID = sendAndGetTicket(message);
            message.SendDate = DateTime.Now.TrimToSeconds();
            message.SendState = SendState.Sent;
            message.State = SMSMessageState.Sent;
            message.Save();
        }

        public static List<SMSMessageDN> CreateAndSendMultipleSMSMessages(CreateMessageParams template, List<string> phones)
        {
            return CreateAndSendMultipleSMSMessages(template, phones, SMSMultipleSendAction);
        }

        //Allows concurrent custom sendProviders for one application
        public static List<SMSMessageDN> CreateAndSendMultipleSMSMessages(CreateMessageParams template,
            List<string> phones, Func<CreateMessageParams, List<string>, List<string>> send)
        {
            var messages = new List<SMSMessageDN>();
            var IDs = send(template, phones);
            var sendDate = DateTime.Now.TrimToSeconds();
            for (int i = 0; i < phones.Count; i++)
            {
                var message = new SMSMessageDN { Message = template.Message, From = template.From }; 
                message.SendDate = sendDate;
                message.SendState = SendState.Sent;
                message.DestinationNumber = phones[i];
                message.MessageID = IDs[i];
                message.Save();
                messages.Add(message);
            }

            return messages;
        }

        public static void UpdateMessageStatus(SMSMessageDN message)
        {
            if (SMSUpdateStatusAction == null)
                throw new InvalidOperationException("SMSUpdateStatusAction was not established");
            UpdateMessageStatus(message, SMSUpdateStatusAction);
        }

        //Allows concurrent custom updateStatusProviders for one application
        public static void UpdateMessageStatus(SMSMessageDN message, Func<SMSMessageDN, SendState> updateAction)
        {
            message.SendState = updateAction(message);
        }

    }

    public class SMSMessageGraph : Graph<SMSMessageDN, SMSMessageState>
    {
        static bool registered;
        public static bool Registered { get { return registered; } }

        public static void Register()
        {
            GetState = m => m.State;

            new ConstructFrom<SMSTemplateDN>(SMSMessageOperations.Create)
            {
                CanConstruct = t => !t.Active ? Resources.TheTemplateMustBeActiveToConstructSMSMessages : null,
                ToState = SMSMessageState.Created,
                Construct = (t, args) =>
                {
                    var message = t.CreateStaticSMSMessage();
                    message.DestinationNumber = args.TryGetArgC<string>(0);
                    return message;
                }
            }.Register();

            new Execute(SMSMessageOperations.Send)
            {
                AllowsNew = true,
                Lite = false,
                FromStates = new[] { SMSMessageState.Created },
                ToState = SMSMessageState.Sent,
                Execute = (t, args) =>
                {
                    var func = args.TryGetArgC<Func<SMSMessageDN, string>>(0);
                    if (func != null)
                        SMSLogic.SendSMS(t, func);
                    else
                        SMSLogic.SendSMS(t);
                }
            }.Register();

            new Execute(SMSMessageOperations.UpdateStatus)
            {
                FromStates = new[] { SMSMessageState.Sent },
                ToState = SMSMessageState.Sent,
                Execute = (t, args) =>
                {
                    var func = args.TryGetArgC<Func<SMSMessageDN, SendState>>(0);
                    if (func != null)
                        SMSLogic.UpdateMessageStatus(t, func);
                    else
                        SMSLogic.UpdateMessageStatus(t);
                }
            }.Register();

            registered = true;
        }
    }

}
