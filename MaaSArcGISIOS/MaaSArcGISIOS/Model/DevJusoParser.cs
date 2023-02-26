using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Model
{
    public class CommonJuso
    {
        public string errorCode { get; set; }
        public string currentPage { get; set; }
        public string totalCount { get; set; }
        public string countPerPage { get; set; }
        public string errorMessage { get; set; }
    }

    public class Juso
    {
        public string rn { get; set; }
        public string rnMgtSn { get; set; }
        public string mtYn { get; set; }
        public string admCd { get; set; }
        public string liNm { get; set; }
        public string emdNm { get; set; }
        public string udrtYn { get; set; }
        public string siNm { get; set; }
        public string lnbrMnnm { get; set; }
        public string sggNm { get; set; }
        public string bdKdcd { get; set; }
        public string buldSlno { get; set; }
        public string zipNo { get; set; }
        public string lnbrSlno { get; set; }
        public string roadAddr { get; set; }
        public string jibunAddr { get; set; }
        public string korAddr { get; set; }
        public string buldMnnm { get; set; }
    }

    public class Results
    {
        public CommonJuso common { get; set; }
        public List<Juso> juso { get; set; }
    }

    public class DevJusoParser
    {
        public Results results { get; set; }
    }
}
