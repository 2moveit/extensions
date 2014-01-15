﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities;
using System.Linq.Expressions;
using Signum.Utilities;
using Signum.Entities.Files;

namespace Signum.Entities.Mailing
{
    [Serializable, EntityKind(EntityKind.Shared, EntityData.Master)]
    public class SmtpConfigurationDN : Entity
    {
        bool isDefault;
        public bool IsDefault
        {
            get { return isDefault; }
            set { Set(ref isDefault, value, () => IsDefault); }
        }

        int port = 25;
        public int Port
        {
            get { return port; }
            set { Set(ref port, value, () => Port); }
        }

        string host;
        public string Host
        {
            get { return host; }
            set { Set(ref host, value, () => Host); }
        }

        bool useDefaultCredentials = true;
        public bool UseDefaultCredentials
        {
            get { return useDefaultCredentials; }
            set { Set(ref useDefaultCredentials, value, () => UseDefaultCredentials); }
        }

        string username;
        public string Username
        {
            get { return username; }
            set { Set(ref username, value, () => Username); }
        }

        string password;
        public string Password
        {
            get { return password; }
            set { Set(ref password, value, () => Password); }
        }

        [NotNullable]
        EmailAddressDN defaultFrom;
        [NotNullValidator]
        public EmailAddressDN DefaultFrom
        {
            get { return defaultFrom; }
            set { Set(ref defaultFrom, value, () => DefaultFrom); }
        }

        [NotNullable]
        MList<EmailRecipientDN> aditionalRecipients = new MList<EmailRecipientDN>();
        [NotNullValidator, NoRepeatValidator]
        public MList<EmailRecipientDN> AditionalRecipients
        {
            get { return aditionalRecipients; }
            set { Set(ref aditionalRecipients, value, () => AditionalRecipients); }
        }

        bool enableSSL;
        public bool EnableSSL
        {
            get { return enableSSL; }
            set { Set(ref enableSSL, value, () => EnableSSL); }
        }

        [NotNullable]
        MList<ClientCertificationFileDN> clientCertificationFiles = new MList<ClientCertificationFileDN>();
        public MList<ClientCertificationFileDN> ClientCertificationFiles
        {
            get { return clientCertificationFiles; }
            set { Set(ref clientCertificationFiles, value, () => ClientCertificationFiles); }
        }

        public override string ToString()
        {
            return "{0} ({1})".Formato(Username, Host);
        }
    }


    public enum SmtpConfigurationOperation
    {
        Save
    }

    [Serializable]
    public class ClientCertificationFileDN : EmbeddedEntity
    {
        [NotNullable, SqlDbType(Size = 300)]
        string fullFilePath;
        [StringLengthValidator(AllowNulls = false, Min = 2, Max = 300), ]
        public string FullFilePath
        {
            get { return fullFilePath; }
            set { Set(ref fullFilePath, value, () => FullFilePath); }
        }

        CertFileType certFileType;
        public CertFileType CertFileType
        {
            get { return certFileType; }
            set { Set(ref certFileType, value, () => CertFileType); }
        }

        static readonly Expression<Func<ClientCertificationFileDN, string>> ToStringExpression = e => e.fullFilePath;
        public override string ToString()
        {
            return ToStringExpression.Evaluate(this);
        }
    }

    public enum CertFileType
    { 
        CertFile,
        SignedFile
    }
}
