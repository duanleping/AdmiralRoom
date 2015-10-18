﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Fleet : GameObject<getmember_deck>, IIdentifiable
    {

        public int Id => rawdata.api_id;
        public string Name => rawdata.api_name;
        public bool CanMission => Id != 1;
        public FleetMissionState MissionState => (FleetMissionState)rawdata.api_mission[0];
        public int MissionID => (int)rawdata.api_mission[1];
        public DateTime BackTime { get; private set; }
        public DateTime BackTimeLocal => BackTime.ToLocalTime();
        public TimeSpan BackTimeRemain => BackTime.Remain();

        #region Ships
        private ObservableCollection<Ship> _ships = new ObservableCollection<Ship>();
        public ObservableCollection<Ship> Ships
        {
            get { return _ships; }
            set
            {
                if (_ships != value)
                {
                    _ships = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public Fleet() { }
        public Fleet(getmember_deck api) : base(api) { }
        public enum FleetMissionState { None = 0, InMission = 1, Complete = 2, Abort = 3 }

        protected override void UpdateProp()
        {
            BackTime = DateTimeHelper.FromUnixTime(rawdata.api_mission[2]);
            //var ships = new List<Ship>();
            bool needupdate = false;
            for(int i = 0; i < rawdata.api_ship.Length; i++)
            {
                if (rawdata.api_ship[i] == -1)
                {
                    if(Ships.Count > i)
                    {
                        needupdate = true;
                        break;
                    }
                }
                else
                {
                    if (Ships.Count <= i || Ships[i].Id != rawdata.api_ship[i] || Ships[i].InFleet != this) 
                    {
                        needupdate = true;
                        break;
                    }
                }
            }
            //foreach (int shipid in rawdata.api_ship)
            //{
            //    if (shipid != -1)
            //    {
            //        ships.Add(Staff.Current.Homeport.Ships[shipid]);
            //        Staff.Current.Homeport.Ships[shipid].InFleet = this;
            //    }
            //}
            if (needupdate)
                Ships = new ObservableCollection<Ship>(rawdata.api_ship.ArrayOperation(x =>
                {
                    if (x == -1) return null;
                    Staff.Current.Homeport.Ships[x].InFleet = this;
                    return Staff.Current.Homeport.Ships[x];
                }));
        }
    }
}