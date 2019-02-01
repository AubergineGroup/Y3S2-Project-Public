using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Sem2_WebApp
{
    public class ChatHub : Hub
    {
        #region snippet_SendMessage

        public async Task SendMessage(int updateFrequency, string fanMode, int fanThreshold, int userThreshold,
            int gasValueThreshold, int requestThreshold)
        {
            await Clients.All.SendAsync("ReceiveMessage", updateFrequency, fanMode, fanThreshold, userThreshold,
                gasValueThreshold, requestThreshold);
        }

        public async Task UpdateTable()
        {
            await Clients.All.SendAsync("ReloadTable");
        }

        #endregion
    }
}