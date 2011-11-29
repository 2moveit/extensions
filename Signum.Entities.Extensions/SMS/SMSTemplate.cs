﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities;
using Signum.Entities.Extensions.Properties;
using System.Linq.Expressions;
using Signum.Utilities;

namespace Signum.Entities.SMS
{

    public enum SMSTemplateState
    {
        Created,
        Modified
    }

    public enum SMSTemplateOperations
    { 
        Create,
        Save,
        Disable,
        Enable
    }

    [Serializable]
    public class SMSTemplateDN : Entity
    {
        [SqlDbType(Size = 100)]
        string name;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 100)]
        public string Name
        {
            get { return name; }
            set { Set(ref name, value, () => Name); }
        }

        TypeDN associatedType;
        public TypeDN AssociatedType
        {
            get { return associatedType; }
            set { Set(ref associatedType, value, () => AssociatedType); }
        }

        string message;
        [StringLengthValidator(AllowNulls = false, Max = SMSCharacters.TripleSMSMaxTextLength)]
        public string Message
        {
            get { return message; }
            set { Set(ref message, value, () => Message); }
        }

        string from = SMSMessageDN.DefaultFrom;
        [StringLengthValidator(AllowNulls = false)]
        public string From
        {
            get { return from; }
            set { Set(ref from, value, () => From); }
        }

        MessageLengthExceeded messageLengthExceeded = MessageLengthExceeded.NotAllowed;
        public MessageLengthExceeded MessageLengthExceeded
        {
            get { return messageLengthExceeded; }
            set { Set(ref messageLengthExceeded, value, () => MessageLengthExceeded); }
        }

        bool removeNoSMSCharacters = true;
        public bool RemoveNoSMSCharacters
        {
            get { return removeNoSMSCharacters; }
            set { Set(ref removeNoSMSCharacters, value, () => RemoveNoSMSCharacters); }
        }

        SMSTemplateState state = SMSTemplateState.Created;
        public SMSTemplateState State
        {
            get { return state; }
            set { Set(ref state, value, () => State); }
        }

        bool active;
        public bool Active
        {
            get { return active; }
            set { Set(ref active, value, () => Active); }
        }

        DateTime startDate = TimeZoneManager.Now.TrimToMinutes();
        [MinutesPrecissionValidator]
        public DateTime StartDate
        {
            get { return startDate; }
            set { Set(ref startDate, value, () => StartDate); }
        }

        DateTime? endDate;
        [MinutesPrecissionValidator]
        public DateTime? EndDate
        {
            get { return endDate; }
            set { Set(ref endDate, value, () => EndDate); }
        }

        static Expression<Func<SMSTemplateDN, bool>> IsActiveNowExpression =
            (mt) => mt.active && DateTime.Now.IsInInterval(mt.StartDate, mt.EndDate);
        public bool IsActiveNow()
        { 
            return IsActiveNowExpression.Evaluate(this);
        }

        protected override string PropertyValidation(System.Reflection.PropertyInfo pi)
        {
            if (pi.Is(() => StartDate) || pi.Is(() => EndDate))
            {
                if (EndDate != null && EndDate >= StartDate)
                    return Resources.EndDateMustBeHigherThanStartDate;
            }

            return base.PropertyValidation(pi);
        }

        public override string ToString()
        {
            return Name;
        }

        public SMSMessageDN CreateStaticSMSMessage()
        {
            return CreateStaticSMSMessage(null);
        }

        public SMSMessageDN CreateStaticSMSMessage(string destinationNumber) 
        {
            return new SMSMessageDN 
            { 
                Template = this.ToLite(),
                Message = this.message,
                From = this.from,
                State = SMSMessageState.Created,
                DestinationNumber = destinationNumber
            };            
        }
    }

    public enum MessageLengthExceeded
    { 
        NotAllowed,
        Allowed,
        TextPruning,
    }

}
