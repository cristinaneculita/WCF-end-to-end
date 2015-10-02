using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoLib.Contracts;
using GeoLib.Data;
using GeoLib.Core;


namespace GeoLib.Services
{
    public class GeoManager :IGeoService

    {
        public GeoManager()
        {
        }

        public GeoManager(IZipCodeRepository zipCodeRepository)
        {
            _zipCodeRespository = zipCodeRepository;
        }

        public GeoManager(IStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }
        public GeoManager(IZipCodeRepository zipCodeRepository, IStateRepository stateRepository)
        {
            _zipCodeRespository = zipCodeRepository;
            _stateRepository = stateRepository;
        }
        private IZipCodeRepository _zipCodeRespository = null;
        private IStateRepository _stateRepository = null;

        public ZipCodeData GetZipInfo(string zip)
        {
            ZipCodeData zipCodeData = null;

            IZipCodeRepository zipCodeRepository = _zipCodeRespository ?? new ZipCodeRepository();

            ZipCode zipCodeEntity = zipCodeRepository.GetByZip(zip);

            if (zipCodeEntity != null)
            {
                zipCodeData = new ZipCodeData()
                {
                    ZipCode = zipCodeEntity.Zip,
                    City = zipCodeEntity.City,
                    State = zipCodeEntity.State.Abbreviation
                };
            }
            return zipCodeData;
        }

        public IEnumerable<string> GetStates(bool primaryOnly)
        {
            List<string> stateData = new List<string>();

            IStateRepository stateRepository = _stateRepository ?? new StateRepository();

            IEnumerable<State> states = stateRepository.Get(primaryOnly);

            if (states !=null)
            {
                foreach (var state in states)
                {
                    stateData.Add(state.Abbreviation);
                }
            }


            return stateData;
        }

        public IEnumerable<ZipCodeData> GetZips(string state)
        {
            List<ZipCodeData> zipCodeData = new List<ZipCodeData>();

            IZipCodeRepository zipCodeRepository = _zipCodeRespository ?? new ZipCodeRepository();

            var zips = zipCodeRepository.GetByState(state);

            if (zips != null)
            {
                foreach (var zipCode in zips)
                {
                    zipCodeData.Add( new ZipCodeData()
                    {
                        ZipCode = zipCode.Zip,
                        City = zipCode.City,
                        State = zipCode.State.Abbreviation
                    });
                }
            }

            return zipCodeData;
        }

        public IEnumerable<ZipCodeData> GetZips(string zip, int range)
        {
            List<ZipCodeData> zipCodeData = new List<ZipCodeData>();

            IZipCodeRepository zipCodeRepository = _zipCodeRespository ?? new ZipCodeRepository();

            ZipCode zipEntity = zipCodeRepository.GetByZip(zip);

            IEnumerable<ZipCode> zips = zipCodeRepository.GetZipsForRange(zipEntity, range);


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
            return zipCodeData;
        }
    }
}
