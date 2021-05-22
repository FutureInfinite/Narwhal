using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Narwhal.Service.SignalR
{
    public class RefreshHub : Hub
    {
        //SignalR hubs are created when needed and possibility of multiple
        //hubs active at one time is possible. To limit instances only one (singleton) 
        //of associated objects in hub will be used
        #region Properties & Attributes                
        private static Task RefreshTask = null;
        private static Task ProcessTask = null;
        private static AutoResetEvent[] RefreshEvents;        

        private enum RefreshEventTypes
        {
            NewDataAvilable = 0,
            RequestStop = 1            
        }

        #endregion //Properties & Attributes

        #region Lifetime
        RefreshHub()
        {
            PrepareNotificationThread();
        }
        #endregion //Lifetime

        #region Operations        
        public void SetDataAvailable()
        {
            RefreshEvents[(int)RefreshEventTypes.NewDataAvilable].Set();
        }

        public override Task OnConnectedAsync()
        {
            try
            {
            }
            catch { }

                        
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }


        private void PrepareNotificationThread()
        {
            if (RefreshEvents == null)
            {
                RefreshEvents = new AutoResetEvent[]
                {
                    new AutoResetEvent(false),      //FIRST EVENT IS FOR NEW DATA NOTIFICATION                 
                    new AutoResetEvent(false)       //STOPPING OF SERCIE - SHUTDOWN
                };
               
            }

            if (RefreshTask == null)
            {
                RefreshTask = Task.Factory.StartNew(
                                () =>
                                {
                                    int nWaitEventIndex = -1;
                                    bool Continue = true;


                                    while (Continue)
                                    {
                                        try
                                        {
                                            nWaitEventIndex = WaitHandle.WaitAny(RefreshEvents, new TimeSpan(0, 5, 0), false);

                                            switch ((RefreshEventTypes)nWaitEventIndex)
                                            {
                                                case RefreshEventTypes.NewDataAvilable:
                                                    RefreshEvents[(int)RefreshEventTypes.NewDataAvilable].Reset();
                                                    Clients.All.SendAsync("NewDataAvailable");                                                    
                                                    break;
                                                case RefreshEventTypes.RequestStop:
                                                    Continue = false;
                                                    break;
                                            }
                                        }
                                        catch
                                        {
                                            //log failure someplace for review / debugging
                                        }
                                    }                                    
                                }
                            );
            }
        }                
        #endregion Operations

    }
}
