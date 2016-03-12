﻿using System;
using System.Diagnostics;
using PushSharp;
using PushSharp.Android;
using PushSharp.Apple;

using System.IO;
using System.Web;
using Microsoft.Owin.Logging;
using PushSharp.Core;


namespace KompetansetorgetServer.PushNotifications
{
    public class Push
    {
        public void PushToAndroid()
        {
            //Create our push services broker
            var push = new PushBroker();

            //Wire up the events for all the services that the broker registers
            push.OnNotificationSent += NotificationSent;
            push.OnChannelException += ChannelException;
            push.OnServiceException += ServiceException;
            push.OnNotificationFailed += NotificationFailed;
            push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
            push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
            push.OnChannelCreated += ChannelCreated;
            push.OnChannelDestroyed += ChannelDestroyed;

            string myAuthToken =
            "d8ERDetXE0g:APA91bGt1yIN4LVYcdZczBDtATDxzCZBZswA6VIDVlXrThNeuAe_7ATdhuG77KBKWoBdi5lkmqs4sp8G4y_3HLGKYyIHlqBDBpuZQ9ZXpK3--1xEiW8P4TJOR-OFmZ47v4HEHfg2VdzL";

            push.RegisterGcmService(new GcmPushChannelSettings("AIzaSyDIbpRonx7yh3NKBAr4rAzmfmIFeEWRTfE"));

            push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(myAuthToken)
                                  .WithJson(@"{""message"":""Hello World!"",""badge"":7,""sound"":""sound.caf""}"));


            //Stop and wait for the queues to drains before it dispose 
            push.StopAllServices();
        }

        static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
            Debug.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
        }

        static void NotificationSent(object sender, INotification notification)
        {
            Debug.WriteLine("Sent: " + sender + " -> " + notification);
        }

        static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            Debug.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
        }

        static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            Debug.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        static void ServiceException(object sender, Exception exception)
        {
            Debug.WriteLine("Service Exception: " + sender + " -> " + exception);
        }

        static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            Debug.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        static void ChannelDestroyed(object sender)
        {
            Debug.WriteLine("Channel Destroyed for: " + sender);
        }

        static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            Debug.WriteLine("Channel Created for: " + sender);
        }
    }

}

/*
public class Push
{
    private PushBroker pushBroker = HttpContext.Current.Application["MyPushBroker"] as PushBroker;
    /*
    public Push()
    {
        //Registering the GCM Service and sending an Android Notification

        try
        {
            pushBroker.RegisterGcmService(new GcmPushChannelSettings("AIzaSyDIbpRonx7yh3NKBAr4rAzmfmIFeEWRTfE"));
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            Debug.WriteLine("FÅR IKKE REGISTERRT!!!");
            Debug.WriteLine("FÅR IKKE REGISTERRT!!!");
            Debug.WriteLine("FÅR IKKE REGISTERRT!!!");

        }
        string myAuthToken =
            "d8ERDetXE0g:APA91bGt1yIN4LVYcdZczBDtATDxzCZBZswA6VIDVlXrThNeuAe_7ATdhuG77KBKWoBdi5lkmqs4sp8G4y_3HLGKYyIHlqBDBpuZQ9ZXpK3--1xEiW8P4TJOR-OFmZ47v4HEHfg2VdzL";

        //Registering the GCM Service and sending an Android Notification
//          pushBroker.RegisterGcmService(new GcmPushChannelSettings("AIzaSyDIbpRonx7yh3NKBAr4rAzmfmIFeEWRTfE"));

    }
    */
/*
/// <summary>
/// Pushes to a hardcoded device
/// </summary>
public void PushToAndroid()
{

    string myAuthToken =
        "dRoHtQTtEdo:APA91bH0KuEL90_iSX6gFXJy2kOPcmDbX_3Ts1-Can3tBIpyWGdipZefLxZayb2zBz93o_8uGMpOOSXxnsSoqO2YAHl9pfh2BYIqm6mbIW71AhXpeQgycbVWj3QnFt9TTgb-2sIKH-qR";

    try
    {
        //Registering the GCM Service
        pushBroker.RegisterGcmService(new GcmPushChannelSettings("AIzaSyDIbpRonx7yh3NKBAr4rAzmfmIFeEWRTfE"));
    }
    catch (Exception)
    {
        Debug.WriteLine("Already registered the GCM Service");
    }    

    pushBroker.QueueNotification(new GcmNotification().ForDeviceRegistrationId(myAuthToken)
        .WithJson(@"{""message"":""Hello World!"",""badge"":7,""sound"":""sound.caf""}"));
    //.WithJson("{\"alert\":\"Hello World!\",\"badge\":7,\"sound\":\"sound.caf\"}"));

}

//Fluent construction of an Android GCM Notification
    //IMPORTANT: For Android you MUST use your own RegistrationId here that gets generated within your Android app itself!



    /*
    //Registering the Apple Service and sending an iOS Notification
    var appleCert = File.ReadAllBytes (
        Path.Combine (AppDomain.CurrentDomain.BaseDirectory, 
            "ApnsSandboxCert.p12"));
    var appleSettings = new ApplePushChannelSettings (appleCert,  
        "password");
    pushBroker.RegisterAppleService(appleSettings);
    var appleNotification = new AppleNotification ()
        .ForDeviceToken ("Device Token HERE")
        .WithAlert ("Hello world!")
        .WithBadge (3)
        .WithSound("sound.caf");
    pushBroker.QueueNotification(appleNotification);
    */


