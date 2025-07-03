using UnityEngine;
using Unity.Netcode;

namespace Player
{
    /**
    * Interface for all network-aware pawns.
    * Any pawn that participates in Netcode networking must implement this interface
    * to expose ownership and network object references.
    *
    * Date Created: 26-06-2025
    * Created By: Prayas Bharadwaj
    *
    * Date Modified: "Latest Modification Date must be mentioned"
    * Modified By: "Latest Modification Author name"
    */
    public interface INetworkPawn
    {
        public NetworkObject NetObject { get; }
        public bool IsOwner { get; }
    }
}