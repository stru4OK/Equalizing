using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Equalizing
{
    public class ViewModel : ViewModelBase
    {
        public ObservableCollection<string> ConfigProfiles { get; private set; }
        Profiles[] Profiles = new Profiles[] { };

        private string dataBase = String.Empty, serverAddr = String.Empty;

        private string _cardNumBorder = "#FFACCD84", _terminalCodeBorder = "#FFACCD84", _dateBorder = "#FFACCD84";
        private string _dateEqualizing = DateTime.Now.ToString(), _cardNumEqualizing = String.Empty, 
            _terminalCodeEqualizing = String.Empty, _redmineTicket = "Задача-", 
            _title = "Equalizing v." + Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private double _billSumEqualizing = 0.00, _spendBonusEqualizing = 0.00, _earnBonusEqualizing = 0.00, _organizerFeeEqualizing = 0.00;

        private int _selectItem = 0;
        private bool _isEnabledUpdate = true;

        private ICommand _Update, _createEqualizing;

        public ViewModel()
        {
            switch(UpdateMethods.Update())
            {
                case 0:
                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                    break;
                case 2:
                    _title += " (Last version)";
                    break;
                default:
                    break;
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

        public string title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                RaisePropertyChanged(() => title);
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
        
        public string dateBorder
        {
            get
            {
                return _dateBorder;
            }
            set
            {
                _dateBorder = value;
                RaisePropertyChanged(() => dateBorder);
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
                _cardNumEqualizing = value.Trim();
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
                _terminalCodeEqualizing = value.Trim();
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
                    bool cardNumInput = false, terminalCodeInput = false, dateInput = false;

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

                    if(Utility.isCorrectDate(dateEqualizing))
                    {
                        dateInput = true;
                        dateBorder = "#FFACCD84";
                    }
                    else
                    {
                        dateInput = false;
                        dateBorder = "Red";
                    }

                    if (cardNumInput && terminalCodeInput && dateInput)
                    {
                        result = DBMethods.TestConnectionDataBase(dataBase);
                        
                        if (!String.IsNullOrEmpty(result))
                            MessageBox.Show(result);
                        else
                        {
                            result = DBMethods.TestEqualizingData(dataBase, terminalCodeEqualizing, cardNumEqualizing, spendBonusEqualizing.ToString().Replace(',', '.'));

                            if (!String.IsNullOrEmpty(result))
                                MessageBox.Show(result, "Ошибка");
                            else
                            {
                                result = EqualizingProcess.CreateEqualizingAndConfirm(dataBase, serverAddr, redmineTicket, dateEqualizing, cardNumEqualizing, terminalCodeEqualizing, billSumEqualizing.ToString().Replace(',', '.'), 
                                    spendBonusEqualizing.ToString().Replace(',', '.'), earnBonusEqualizing.ToString().Replace(',', '.'), organizerFeeEqualizing.ToString().Replace(',', '.'));

                                MessageBox.Show(result, "Инфо");

                                cardNumEqualizing = String.Empty;
                            }
                        }
                    }
                }));
            }
        }
    }
}
