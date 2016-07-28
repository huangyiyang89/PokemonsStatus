#region

using System;
using System.Collections.Generic;
using System.IO;
using PokemonGo.RocketAPI.Enums;

using System.Text.RegularExpressions;
using PokemonGo.RocketAPI;

#endregion

namespace PokemonsStatus
{
    public class Settings : ISettings
    {
        
       

        private AuthType authType;
        private string ptcUsername;
        private string ptcPassword;
        private double defaultLatitude;
        private double defaultLongitude;
        private double defaultAltitude;
        private string googleUsername;
        private string googlePassword;
        

        public AuthType AuthType
        {
            get
            {
                return authType;
            }

            set
            {
                authType = value;
            }
        }

        public string PtcUsername
        {
            get
            {
                return ptcUsername;
            }

            set
            {
                ptcUsername = value;
            }
        }

        public string PtcPassword
        {
            get
            {
                return ptcPassword;
            }

            set
            {
                ptcPassword = value;
            }
        }

        public double DefaultLatitude
        {
            get
            {
                return defaultLatitude;
            }

            set
            {
                defaultLatitude = value;
            }
        }

        public double DefaultLongitude
        {
            get
            {
                return defaultLongitude;
            }

            set
            {
                defaultLongitude = value;
            }
        }

        

        public string GoogleRefreshToken
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

      

        public double DefaultAltitude
        {
            get
            {
                return defaultAltitude;
            }

            set
            {
                defaultAltitude = value;
            }
        }

        public string GoogleUsername
        {
            get
            {
                return googleUsername;
            }

            set
            {
                googleUsername = value;
            }
        }

        public string GooglePassword
        {
            get
            {
                return googlePassword;
            }

            set
            {
                googlePassword = value;
            }
        }
    }
}
