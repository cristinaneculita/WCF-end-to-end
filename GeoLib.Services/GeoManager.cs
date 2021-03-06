﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoLib.Contracts;
using GeoLib.Data;
using GeoLib.Core;


namespace GeoLib.Services
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, ReleaseServiceInstanceOnTransactionComplete = false)]
    public class GeoManager : IGeoService, IGeoAdminService

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
            //throw new DivideByZeroException("you cannot try this");
            ZipCodeData zipCodeData = null;

            string hostIdentity = WindowsIdentity.GetCurrent().Name;
            string primaryIdentity = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            string windowsIdentity = ServiceSecurityContext.Current.WindowsIdentity.Name;
            string threadIdentity = Thread.CurrentPrincipal.Identity.Name;


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
            else
            {
                //throw new ApplicationException($"Zip code {zip} not found.");
                //throw new FaultException($"Zip code {zip} not found.");
                //ApplicationException ex = new ApplicationException($"Zip code {zip} not found.");
                //throw new FaultException<ApplicationException>(ex, "Just another message");

                NotFoundData data = new NotFoundData()
                {
                    Message = string.Format($"Zip code {zip} not found."),
                    When = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    User = "miguel"
                };
                throw new FaultException<NotFoundData>(data, "No reason");
            }

            return zipCodeData;
        }

        public IEnumerable<string> GetStates(bool primaryOnly)
        {
            List<string> stateData = new List<string>();

            IStateRepository stateRepository = _stateRepository ?? new StateRepository();

            IEnumerable<State> states = stateRepository.Get(primaryOnly);

            if (states != null)
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




        //public void UpdateZipCity(string zip, string city)
        //{
        //    IZipCodeRepository zipCodeRepository = _zipCodeRespository ?? new ZipCodeRepository();

        //    ZipCode zipEntity = zipCodeRepository.GetByZip(zip);
        //    if (zipEntity != null)
        //    {
        //        zipEntity.City = city;
        //        zipCodeRepository.Update(zipEntity);
        //    }
        //}
        [OperationBehavior(TransactionScopeRequired = true)]
        public void UpdateZipCity(string zip, string city)
        {
            IZipCodeRepository zipCodeRepository = _zipCodeRespository ?? new ZipCodeRepository();

            ZipCode zipEntity = zipCodeRepository.GetByZip(zip);
            if (zipEntity != null)
            {
                zipEntity.City = city;
                zipCodeRepository.Update(zipEntity);
            }
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        [PrincipalPermission(SecurityAction.Demand, Role ="Administrators")]
        public void UpdateZipCity(IEnumerable<ZipCityData> zipCityData)
        {
            string hostIdentity = WindowsIdentity.GetCurrent().Name;
            string primaryIdentity = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            string windowsIdentity = ServiceSecurityContext.Current.WindowsIdentity.Name;
            string threadIdentity = Thread.CurrentPrincipal.Identity.Name;

            IZipCodeRepository zipCodeRepository = _zipCodeRespository ?? new ZipCodeRepository();

            Dictionary<string, string> cityBatch = new Dictionary<string, string>();

            foreach (ZipCityData zipCityItem in zipCityData)
                cityBatch.Add(zipCityItem.ZipCode, zipCityItem.City);

            zipCodeRepository.UpdateCityBatch(cityBatch);

        }

    }
}
