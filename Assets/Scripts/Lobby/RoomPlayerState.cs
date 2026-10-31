using System;
using Unity.Collections;
using Unity.Netcode;

public struct RoomPlayerState : INetworkSerializable, IEquatable<RoomPlayerState>
{
    public ulong ClientId;
    public FixedString64Bytes PlayerName;
    public FixedString64Bytes JobId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref JobId);
    }

    public bool Equals(RoomPlayerState other)
    {
        return
            ClientId == other.ClientId &&
            PlayerName == other.PlayerName &&
            JobId == other.JobId;
    }
}