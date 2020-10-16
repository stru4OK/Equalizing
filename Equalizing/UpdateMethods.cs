using System;
using System.IO;
using System.Net;
using System.Windows;

namespace Equalizing
{
    public class UpdateMethods
    {
        public static int Update()
        {
            WebClient client = new WebClient();

            bool isUpdate = false;

            string exeFile = "Equalizing.exe";
            string profileFile = "Profiles.jsn";

            string profilesFileURL = "http://192.168.4.150:90/UpdateUtils/" + profileFile;
            string exeFileURL = "http://192.168.4.150:90/UpdateUtils/equalizing/" + exeFile;

            if (RemoteFileExists(profilesFileURL))
            {
                if (isNeedUpdateRemoteFile(profilesFileURL, profileFile))
                {
                    client.DownloadFile(profilesFileURL, profileFile);
                    isUpdate = true;
                }

                if (RemoteFileExists(exeFileURL))
                {
                    if (isNeedUpdateRemoteFile(exeFileURL, exeFile))
                    {
                        if (File.Exists(exeFile + "_old"))
                            File.Delete(exeFile + "_old");

                        File.Move(exeFile, exeFile + "_old");

                        client.DownloadFile(exeFileURL, exeFile);
                        isUpdate = true;
                    }
                }

                if (isUpdate)
                {
                    //MessageBox.Show("Обновление прошло успешно!\nТребуется рестарт приложения", "Сообщение");
                    //MessageBox.Show("Приложение будет закрыто\nпосле успешного обновления!", "Сообщение");
                    //Thread.Sleep(3000);
                    return 0;
                }

                return 2;
            }
            else
            {
                //MessageBox.Show("Проблемы с сервером обновлений!", "Ошибка");
                return 1;
            }
        }

        private static bool RemoteFileExists(string fileUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(fileUrl) as HttpWebRequest;
                request.Method = "HEAD";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool isNeedUpdateRemoteFile(string fileUrl, string localFile)
        {
            try
            {
                HttpWebRequest file = (HttpWebRequest)WebRequest.Create(fileUrl);
                HttpWebResponse fileResponse = (HttpWebResponse)file.GetResponse();

                fileResponse.Close();

                DateTime localFileModifiedTime = File.GetLastWriteTime(localFile);
                DateTime onlineFileModifiedTime = fileResponse.LastModified;

                if (localFileModifiedTime < onlineFileModifiedTime)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
