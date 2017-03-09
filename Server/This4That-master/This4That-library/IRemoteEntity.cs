using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_library
{
    public interface IRemoteEntity
    {
    }

    public interface IServerManager : IRemoteEntity
    {
        bool RegisterTaskCreatorNode(string tcpUrl);
        bool RegisterTaskDistributorNode(string tcpUrl);

    }

    public interface ITaskCreator : IRemoteEntity
    {

    }

    public interface ITaskDistributor : IRemoteEntity
    {

    }
}
