using System.Collections;
using UnityEngine;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Issimissimo.Networking
{
    public class InternetConnection : MonoBehaviour
    {
        public static InternetConnection instance;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }
        
        private string endpoint = "https://google.it/";


        public void CheckAvailability(Action<bool> result)
        {
            StartCoroutine(CheckAvailability_Coroutine(result));
        }


        IEnumerator CheckAvailability_Coroutine(Action<bool> result)
        {
            UnityWebRequest request = UnityWebRequest.Get(endpoint);
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError) // Error
                {
                    if (result != null) result(false);
                }
                else // Success
                {
                    if (result != null) result(true);
                }
            }
        }



        public double GetInternetSpeed()
        {
            // Create Object Of WebClient
            System.Net.WebClient wc = new System.Net.WebClient();

            //DateTime Variable To Store Download Start Time.
            DateTime dt1 = DateTime.UtcNow;

            //Number Of Bytes Downloaded Are Stored In ‘data’
            byte[] data = wc.DownloadData(endpoint);

            //DateTime Variable To Store Download End Time.
            DateTime dt2 = DateTime.UtcNow;

            //To Calculate Speed in Kb Divide Value Of data by 1024 And Then by End Time Subtract Start Time To Know Download Per Second.
            return Math.Round((data.Length / 1024) / (dt2 - dt1).TotalSeconds, 2);
        }


        public async Task<int> GetInternetSpeedAsync(CancellationToken ct = default)
        {
            await Task.Delay(20000);
            
            const double kb = 1024;

            // do not use compression
            using var client = new HttpClient();

            int numberOfBytesRead = 0;

            var buffer = new byte[10240].AsMemory();

            // create request
            var stream = await client.GetStreamAsync("https://www.google.it");

            // start timer
            DateTime dt1 = DateTime.UtcNow;

            // download stuff
            while (true)
            {
                var i = await stream.ReadAsync(buffer, ct);
                if (i < 1)
                    break;

                numberOfBytesRead += i;
            }

            // end timer
            DateTime dt2 = DateTime.UtcNow;

            double kilobytes = numberOfBytesRead / kb;
            double time = (dt2 - dt1).TotalSeconds;
            // speed in Kb per Second.
            Debug.Log("speed:" + (int)(kilobytes / time));
            return (int)(kilobytes / time);
        }
    }
}




