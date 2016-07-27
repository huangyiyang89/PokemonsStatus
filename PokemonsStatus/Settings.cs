#region

using System;
using System.Collections.Generic;
using System.IO;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.GeneratedCode;
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
        public double DefaultAltitude => 10;
       
      
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


    }
}
