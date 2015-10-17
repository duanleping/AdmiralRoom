﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Huoyaoyuan.AdmiralRoom.API;
using Huoyaoyuan.AdmiralRoom.Models;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Ship : GameObject<api_ship>, IIdentifiable
    {
        public Ship() { }
        public Ship(api_ship api) : base(api) { }
        public int Id => rawdata.api_id;
        public int SortNo => rawdata.api_sortno;
        public int ShipId => rawdata.api_ship_id;
        public int Level => rawdata.api_lv;
        public Exp Exp => new Exp(rawdata.api_exp);
        public LimitedValue HP { get; private set; }
        public ShootRange Range => (ShootRange)rawdata.api_leng;

        #region Slots
        private ObservableCollection<Slot> _slots;
        public ObservableCollection<Slot> Slots
        {
            get { return _slots; }
            set
            {
                if (_slots != value)
                {
                    _slots = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        
        public Slot SlotEx { get; private set; }
        public Modernizable Firepower { get; private set; }
        public Modernizable Torpedo { get; private set; }
        public Modernizable AA { get; private set; }
        public Modernizable Armor { get; private set; }
        public Modernizable Luck { get; private set; }
        public LimitedValue Evasion { get; private set; }
        public LimitedValue ASW { get; private set; }
        public LimitedValue ViewRange { get; private set; }
        public int Rare => rawdata.api_backs;

        #region Fuel
        private LimitedValue _fuel;
        public LimitedValue Fuel
        {
            get { return _fuel; }
            set
            {
                _fuel = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Bull
        private LimitedValue _bull;
        public LimitedValue Bull
        {
            get { return _bull; }
            set
            {
                _bull = value;
                OnPropertyChanged();
            }
        }
        #endregion
        public int SlotNum => rawdata.api_slotnum;
        public TimeSpan RepairTime => TimeSpan.FromMilliseconds(rawdata.api_ndock_time);
        public int RepairFuel => rawdata.api_ndock_item[0];
        public int RepairSteel => rawdata.api_ndock_item[1];
        public int MordenizeRate => rawdata.api_srate;
        public int Condition => rawdata.api_cond;
        public bool Locked => rawdata.api_locked != 0;
        public bool LockedEquip => rawdata.api_locked_equip != 0;
        public ShipInfo ShipInfo => Staff.Current.MasterData.ShipInfo[ShipId];

        #region IsRepairing
        private bool _isrepairing;
        public bool IsRepairing
        {
            get { return _isrepairing; }
            set
            {
                if (_isrepairing != value)
                {
                    _isrepairing = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region RepairingHP
        private int _repairingHP;
        public int RepairingHP
        {
            get { return _repairingHP; }
            set
            {
                if (_repairingHP != value)
                {
                    _repairingHP = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        public Fleet InFleet { get; set; }

        protected override void UpdateProp()
        {
            HP = new LimitedValue(rawdata.api_nowhp, rawdata.api_maxhp);
            RepairingHP = HP.Current;
            Evasion = new LimitedValue(rawdata.api_kaihi);
            ASW = new LimitedValue(rawdata.api_taisen);
            ViewRange = new LimitedValue(rawdata.api_sakuteki);
            Firepower = new Modernizable(ShipInfo.FirePower, rawdata.api_kyouka[0], rawdata.api_karyoku[0]);
            Torpedo = new Modernizable(ShipInfo.Torpedo, rawdata.api_kyouka[1], rawdata.api_raisou[0]);
            AA = new Modernizable(ShipInfo.AA, rawdata.api_kyouka[2], rawdata.api_taiku[0]);
            Armor = new Modernizable(ShipInfo.Armor, rawdata.api_kyouka[3], rawdata.api_soukou[0]);
            Luck = new Modernizable(ShipInfo.Luck, rawdata.api_kyouka[4], rawdata.api_lucky[0]);
            Fuel = new LimitedValue(rawdata.api_fuel, ShipInfo.MaxFuel);
            Bull = new LimitedValue(rawdata.api_bull, ShipInfo.MaxBull);
            var slots = new List<Slot>();
            for(int i = 0; i < SlotNum; i++)
            {
                var slot = new Slot();
                if (rawdata.api_slot[i] != -1)
                    slot.Item = Staff.Current.Homeport.Equipments[rawdata.api_slot[i]];
                slots.Add(slot);
            }
            for(int i = 0; i < slots.Count; i++)
            {
                slots[i].AirCraft = new LimitedValue(rawdata.api_onslot[i], ShipInfo.AirCraft[i]);
            }
            Slots = new ObservableCollection<Slot>(slots);
            SlotEx = new Slot();
            if (rawdata.api_slot_ex == 0) SlotEx.IsLocked = true;
            else if (rawdata.api_slot_ex != -1) SlotEx.Item = Staff.Current.Homeport.Equipments[rawdata.api_slot_ex];
        }
    }
    public enum ShootRange { None = 0, Short = 1, Long = 2, VLong = 3 }
}