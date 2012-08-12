﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities;
using Signum.Utilities;

namespace Signum.Entities.Files
{
    public interface IFile 
    {
        byte[] BinaryFile { get; set; }
        string FileName { get; set; }
        string FullWebPath { get; }
    }

    [Serializable]
    public class EmbeddedFileDN : EmbeddedEntity, IFile
    {
        [NotNullable]
        string fileName;
        [StringLengthValidator(Min = 3)]
        public string FileName
        {
            get { return fileName; }
            set { SetToStr(ref fileName, value, () => FileName); }
        }

        [NotNullable]
        byte[] binaryFile;
        [NotNullValidator]
        public byte[] BinaryFile
        {
            get { return binaryFile; }
            set { SetToStr(ref binaryFile, value, () => BinaryFile); }
        }
        
        public override string ToString()
        {
            return "{0} {1}".Formato(fileName, BinaryFile.TryCC(bf => StringExtensions.ToComputerSize(bf.Length)) ?? "??");
        }

        
        public string FullWebPath
        {
            get { return null; }
        }
    }
}
