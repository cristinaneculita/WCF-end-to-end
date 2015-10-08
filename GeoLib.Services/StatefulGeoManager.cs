using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using GeoLib.Contracts;
using GeoLib.Data;

namespace GeoLib.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class StatefulGeoManager : IStatefulGeoService
    {
        private ZipCode _ZipCodeEntity;
        public ZipCodeData GetZipInfo()
        {
            ZipCodeData zipCodeData = null;

            if (_ZipCodeEntity != null)
            {
                zipCodeData = new ZipCodeData()
                {
                    ZipCode = _ZipCodeEntity.Zip,
                    City = _ZipCodeEntity.City,
                    State = _ZipCodeEntity.State.Abbreviation
                };
            }
            else
                throw new ApplicationException("uh oh!");
           
            return zipCodeData;
        }

        public IEnumerable<ZipCodeData> GetZips(int range)
        {
            List<ZipCodeData> zipCodeData = new List<ZipCodeData>();

            if (_ZipCodeEntity != null)

            {
                IZipCodeRepository zipCodeRepository = new ZipCodeRepository();

                IEnumerable<ZipCode> zips = zipCodeRepository.GetZipsForRange(_ZipCodeEntity, range);


                if (zips != null)
                {
                    foreach (var zipCode in zips)
                    {
                        zipCodeData.Add(new ZipCodeData()
                        {
                            ZipCode = zipCode.Zip,
                            City = zipCode.City,
                            State = zipCode.State.Abbreviation
                        });
                    }
                }
            }
            return zipCodeData;
        
    }

        public void PushZip(string zip)
        {
            IZipCodeRepository zipCodeRepository = new ZipCodeRepository();

            _ZipCodeEntity = zipCodeRepository.GetByZip(zip);
        }
    }
}
