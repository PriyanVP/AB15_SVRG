using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AB15_GUI.WPF.Models.Generated.Registers;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Data record class that holds PSI Sensor Data
    /// </summary>
    public class PsiData : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PsiData()
        {
            // Initialize register list
            InitRegisterList();

            // Raise event to update UI with new data
            OnPropertyChanged(nameof(PsiRegisterList));

            // Get addresses for registers
            addresses = PsiRegisterList.Select(x => x.Register.Address).ToList();
        }

        /// <summary>
        /// PSI register list with observability on each element
        /// </summary>
        public List<ObservableRegister> PsiRegisterList { get; set; } = new List<ObservableRegister>();

        /// <summary>
        /// <inheritdoc cref="Addresses" path='/summary'/>
        /// </summary>
        public List<ushort> addresses;
        
        /// <summary>
        /// List with addresses to read PSI data
        /// </summary>
        public List<ushort> Addresses 
        { 
            get
            {
                return addresses;
            }
        }

        /// <summary>
        /// Populate PSI register list
        /// </summary>
        private void InitRegisterList()
        {
            // TODO: can be refactored to use Observable collection method to notify about changes, no need for Observable register than
            
            // Slot 1
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot1_Ch1()));
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot1_Ch2()));
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot1_Ch3()));
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot1_Ch4()));

            // Slot 2
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot2_Ch1()));
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot2_Ch2()));
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot2_Ch3()));
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot2_Ch4()));

            // Slot 3
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot3_Ch1()));
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot3_Ch2()));
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot3_Ch3()));
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot3_Ch4()));

            // Slot 4
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot4_Ch1()));
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot4_Ch2()));
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot4_Ch3()));
            PsiRegisterList.Add(new ObservableRegister(new Reg_PSI_Read_Data_Slot4_Ch4()));
        }

        /// <summary>
        /// Update stored data with new data
        /// </summary>
        /// <param name="data">new register data</param>
        /// <returns>true if update was successful, false - otherwise</returns>
        public bool UpdateData(List<ushort> data)
        {
            // Sanity check
            if (data.Count != PsiRegisterList.Count) return false;

            // Update data in registers
            for (int i = 0; i < data.Count; i++)
            {
                PsiRegisterList[i].Data = data[i];
            }

            return true;
        }

        #region Services

        /// <summary>
        /// Event for notification if property has changed
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raise event to notify about property change
        /// </summary>
        /// <param name="propertyName">name of property</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            // Sanity check
            if (propertyName == null) throw new ArgumentException("Property name can't be null!");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // Services
    }
}
