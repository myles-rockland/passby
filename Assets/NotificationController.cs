using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

namespace PassBy
{
    public class NotificationController : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        void Start()
        {
            StartCoroutine(RequestNotificationPermission());

            var group = new AndroidNotificationChannelGroup()
            {
                Id = "main",
                Name = "Main notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannelGroup(group);

            var channel = new AndroidNotificationChannel()
            {
                Id = "passby",
                Name = "Passby Channel",
                Importance = Importance.Default,
                Description = "Notifications for when the player passes by another player.",
                Group = "main",  // must be same as Id of previously registered group
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }

        public IEnumerator RequestNotificationPermission()
        {
            var request = new PermissionRequest();
            while (request.Status == PermissionStatus.RequestPending)
                yield return null;
            // here use request.Status to determine users response
        }

        public void SendPassbyNotification(string title, string text)
        {
            var notification = new AndroidNotification();
            notification.Title = title;
            notification.Text = text;
            notification.FireTime = System.DateTime.Now;

            AndroidNotificationCenter.SendNotification(notification, "passby");
        }
    }
}
