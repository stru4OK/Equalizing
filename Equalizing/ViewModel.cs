using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Equalizing
{
    public class ViewModel : ViewModelBase
    {
        public ObservableCollection<string> ConfigProfiles { get; private set; }
        Profiles[] Profiles = new Profiles[] { };

        private string dataBase = String.Empty;
        private string serverAddr = String.Empty;

        private string _cardNumBorder = "#FFACCD84";
        private string _terminalCodeBorder = "#FFACCD84";
        private string _dateEqualizing = DateTime.Now.ToString();
        private string _cardNumEqualizing = String.Empty;
        private string _terminalCodeEqualizing = String.Empty;
        private string _redmineTicket = "Задача-";

        private double _billSumEqualizing = 0.00;
        private double _spendBonusEqualizing = 0.00;
        private double _earnBonusEqualizing = 0.00;
        private double _organizerFeeEqualizing = 0.00;

        private int _selectItem = 0;

        private bool _isEnabledUpdate = true;

        private ICommand _Update;
        private ICommand _createEqualizing;

        public ViewModel()
        {
            if (UpdateMethods.Update() == 0)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }

            ConfigProfiles = new ObservableCollection<string> { };
            Profiles = GetProfiles.ReadProfiles();
        
            if (Profiles[0].dataSource.Contains("Data Source"))
                GetProfiles.SaveProfiles(Profiles);

            for (int i = 0; i < Profiles.Length; i++)
                ConfigProfiles.Add(Profiles[i].profileName);
        }

        public int selectItem
        {
            get
            {
                dataBase = Crypto.SimpleDecryptWithPassword(Profiles[_selectItem].dataSource, "2346dfgxdr6fjufcgbjdcgfh");
                serverAddr = Profiles[_selectItem].serverAddr;

                return _selectItem;
            }
            set
            {
                _selectItem = value;
                RaisePropertyChanged(() => selectItem);
            }
        }

        public string redmineTicket
        {
            get
            {
                return _redmineTicket;
            }
            set
            {
                _redmineTicket = value;
                RaisePropertyChanged(() => redmineTicket);
            }
        }
        
        public string cardNumBorder
        {
            get
            {
                return _cardNumBorder;
            }
            set
            {
                _cardNumBorder = value;
                RaisePropertyChanged(() => cardNumBorder);
            }
        }

        public string terminalCodeBorder
        {
            get
            {
                return _terminalCodeBorder;
            }
            set
            {
                _terminalCodeBorder = value;
                RaisePropertyChanged(() => terminalCodeBorder);
            }
        }

        public string dateEqualizing
        {
            get
            {
                return _dateEqualizing;
            }
            set
            {
                _dateEqualizing = value;
                RaisePropertyChanged(() => dateEqualizing);
            }
        }

        public string cardNumEqualizing
        {
            get
            {
                return _cardNumEqualizing;
            }
            set
            {
                _cardNumEqualizing = value;
                RaisePropertyChanged(() => cardNumEqualizing);
            }
        }

        public string terminalCodeEqualizing
        {
            get
            {
                return _terminalCodeEqualizing;
            }
            set
            {
                _terminalCodeEqualizing = value;
                RaisePropertyChanged(() => terminalCodeEqualizing);
            }
        }

        public double billSumEqualizing
        {
            get
            {
                return _billSumEqualizing;
            }
            set
            {
                _billSumEqualizing = value;
                RaisePropertyChanged(() => billSumEqualizing);
            }
        }

        public double spendBonusEqualizing
        {
            get
            {
                return _spendBonusEqualizing;
            }
            set
            {
                _spendBonusEqualizing = value;
                RaisePropertyChanged(() => spendBonusEqualizing);
            }
        }

        public double earnBonusEqualizing
        {
            get
            {
                return _earnBonusEqualizing;
            }
            set
            {
                _earnBonusEqualizing = value;
                RaisePropertyChanged(() => earnBonusEqualizing);
            }
        }

        public double organizerFeeEqualizing
        {
            get
            {
                return _organizerFeeEqualizing;
            }
            set
            {
                _organizerFeeEqualizing = value;
                RaisePropertyChanged(() => organizerFeeEqualizing);
            }
        }

        public bool isEnabledUpdate
        {
            get
            {
                return _isEnabledUpdate;
            }
            set
            {
                _isEnabledUpdate = value;
                RaisePropertyChanged(() => isEnabledUpdate);
            }
        }

        public ICommand Update
        {
            get
            {
                return _Update ?? (_Update = new RelayCommand(() =>
                {
                    if (UpdateMethods.Update() == 0)
                        isEnabledUpdate = false;
                }));
            }
        }

        public ICommand CreateEqualizing
        {
            get
            {
                return _createEqualizing ?? (_createEqualizing = new RelayCommand(() =>
                {
                    string result = String.Empty;
                    bool cardNumInput = false, terminalCodeInput = false;

                    if (!String.IsNullOrEmpty(cardNumEqualizing))
                    {
                        cardNumInput = true;
                        cardNumBorder = "#FFACCD84";
                    }
                    else
                    {
                        cardNumInput = false;
                        cardNumBorder = "Red";
                    }

                    if (!String.IsNullOrEmpty(terminalCodeEqualizing))
                    {
                        terminalCodeInput = true;
                        terminalCodeBorder = "#FFACCD84";
                    }
                    else
                    {
                        terminalCodeInput = false;
                        terminalCodeBorder = "Red";
                    }

                    if (!String.IsNullOrEmpty(terminalCodeEqualizing))
                    {
                        terminalCodeInput = true;
                        terminalCodeBorder = "#FFACCD84";
                    }
                    else
                    {
                        terminalCodeInput = false;
                        terminalCodeBorder = "Red";
                    }
                    
                    if (cardNumInput & terminalCodeInput)
                    {
                        result = DBMethods.TestConnectionDataBase(dataBase);
                        
                        if (!String.IsNullOrEmpty(result))
                            MessageBox.Show(result);
                        else
                        {
                            result = DBMethods.TestEqualizingData(dataBase, terminalCodeEqualizing, cardNumEqualizing, spendBonusEqualizing.ToString().Replace(',', '.'));

                            if (!String.IsNullOrEmpty(result))
                                MessageBox.Show(result);
                            
                            else
                            {
                                result = DBMethods.CreateEqualizing(dataBase, serverAddr, redmineTicket, dateEqualizing, cardNumEqualizing, terminalCodeEqualizing, billSumEqualizing.ToString().Replace(',', '.'), 
                                    spendBonusEqualizing.ToString().Replace(',', '.'), earnBonusEqualizing.ToString().Replace(',', '.'), organizerFeeEqualizing.ToString().Replace(',', '.'));

                                if (!String.IsNullOrEmpty(result))
                                    MessageBox.Show(result);
                                else
                                    MessageBox.Show("Корректировка проведена успешно и подтверждена!");

                                cardNumEqualizing = String.Empty;
                            }
                        }
                    }
                }));
            }
        }
    }
}
