using System;
using Unity.Collections;
using Unity.Netcode;

public struct LeaderBoardEntityState : INetworkSerializable, IEquatable<LeaderBoardEntityState>
{
    public ulong clientID;
    public FixedString32Bytes playerName;
    public int coins;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref coins);
    }

    public bool Equals(LeaderBoardEntityState other)
    {
        return clientID == other.clientID 
               && playerName.Equals(other.playerName) 
               && coins == other.coins;
    }
}