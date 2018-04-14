using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MXApi.Models
{
    public class ProdFileItem : POD_Repository
    {
        private string _podFIleName;
        private PODFileTypes _fileType;
        public string PODFileName
        {
            get
            {
                if (string.IsNullOrEmpty(_podFIleName))
                {
                    PODLink = PODLink.Replace("\\\\", "");
                    PODLink = PODLink.Replace(@"\", @"/");
                    _podFIleName = Path.GetFileNameWithoutExtension(PODLink);
                }
                return _podFIleName;
            }
            set
            {
                _podFIleName = value;
            }
        }
        public PODFileTypes FileType
        {
            get
            {
                if (_fileType == 0)
                {
                    PODLink = PODLink.Replace("\\\\", "");
                    PODLink = PODLink.Replace(@"\", @"/");
                    FileInfo info = new FileInfo(PODLink);
                    if (info.Extension == ".pdf")
                    {
                        return PODFileTypes.PDF;
                    }
                    else if (info.Extension == ".xlsx")
                    {
                        return PODFileTypes.Excel;
                    }
                    else if (info.Extension == ".docx")
                    {
                        return PODFileTypes.Docx;
                    }
                    else if (info.Extension == ".jpeg")
                    {
                        return PODFileTypes.Image;
                    }
                    else
                        return PODFileTypes.Other;
                }
                return _fileType;
            }
            set
            {
                _fileType = value;
            }
        }
    }
    public partial class POD_Repository
    {
        public int PODID { get; set; }
        public string PODKey { get; set; }
        public string PODDescription { get; set; }
        public string PODLink { get; set; }
        public System.DateTime PODBorn { get; set; }
        public string PODCreatedBy { get; set; }
    }
    public enum PODFileTypes
    {
        PDF = 1,
        Excel = 2,
        Docx = 3,
        Image = 4,
        Other = 5
    }
}
