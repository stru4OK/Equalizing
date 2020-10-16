using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace Equalizing
{
    public class GetProfiles
    {
        public static Profiles[] ReadProfiles()
        {
            string profilePath = "Profiles.jsn";
            string profileData = String.Empty;

            Profiles[] Profiles = new Profiles[] { };

            try
            {
                StreamReader file = new StreamReader(profilePath);
                profileData = file.ReadToEnd();

                Profiles = JsonConvert.DeserializeObject<Profiles[]>(profileData);

                return Profiles;
            }
            catch (Exception)
            {
                //MessageBox.Show("Ошибка чтения Profiles.jsn", "Ошибка");
                return Profiles;
            }
        }

        public static void SaveProfiles(Profiles[] Profiles)
        {
            string profilePath = "Profiles.jsn";
            string profileData = String.Empty;

            for(int i = 0; i<Profiles.Length; i++)
                Profiles[i].dataSource = Crypto.SimpleEncryptWithPassword(Profiles[i].dataSource, "2346dfgxdr6fjufcgbjdcgfh");

            try
            {
                profileData = JsonConvert.SerializeObject(Profiles);

                File.WriteAllText(profilePath, profileData);
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка сохранения Profiles.jsn", "Ошибка");
            }
        }
    }

    public class Profiles
    {
        public string profileName { get; set; }
        public string serverAddr { get; set; }
        public string dataSource { get; set; }
    }
}
