using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Xml;

namespace Dolar_Euro_Kur_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GunlukKurController : ControllerBase
    {
        [HttpPost]
        public ResponseData Run(RequestData request)
        {
            ResponseData result = new ResponseData();

            try
            {
                string tcmblink = string.Format("http://www.tcmb.gov.tr/kurlar/{0}.xml",
              (request.IsBugun) ? "today" : string.Format("{2}{1}/{0}{1}{2}",
              request.Gun.ToString().PadLeft(2, '0'), request.Ay.ToString().PadLeft(2, '0'), request.Yıl)
              );

                result.Data = new List<ResponseDataKur>();

                XmlDocument doc = new XmlDocument();

                doc.Load(tcmblink);

                if (doc.SelectNodes("Tarih_Date").Count < 1)
                {
                    result.Hata = "Kur Bilgisi Bulunamadı";
                    return result;
                }
                foreach (XmlNode node in doc.SelectNodes("Tarih_Date")[0].ChildNodes)
                {
                    ResponseDataKur kur = new ResponseDataKur();
                    kur.Kodu = node.Attributes["Kod"].Value;

                    kur.Adi = node["Isim"].InnerText;
                    kur.Birimi = int.Parse(node["Unit"].InnerText);
                    kur.AlisKuru = Convert.ToDecimal("0" + node["ForexBuying"].InnerText.Replace(".", ","));
                    kur.SatisKuru = Convert.ToDecimal("0" + node["ForexSelling"].InnerText.Replace(".", ","));
                    kur.EfektifAlisKuru = Convert.ToDecimal("0" + node["BanknoteBuying"].InnerText.Replace(".", ","));
                    kur.EfektifAlisKuru = Convert.ToDecimal("0" + node["BanknoteSelling"].InnerText.Replace(".", ","));

                    result.Data.Add(kur);

                }
            }
            catch (Exception ex)
            {
                result.Hata = ex.Message;
            }
            return result;
        }
    }
}
