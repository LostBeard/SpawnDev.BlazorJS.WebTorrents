using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;
using System.Numerics;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Bitfield object
    /// </summary>
    public class Bitfield : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Bitfield(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Bitfield buffer
        /// </summary>
        public Uint8Array Buffer => JSRef!.Get<Uint8Array>("buffer");
        /// <summary>
        /// Returns true if the index exists and is 1
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Get(int index) => JSRef!.Call<bool>("get", index);
        /// <summary>
        /// Returns the length of the bitfield
        /// </summary>
        public int Length => JSRef!.Get<int>("length");
        /// <summary>
        /// Percentage of the bitfield that is 1
        /// </summary>
        /// <returns></returns>
        public float Percent()
        {
            float len = Length;
            if (len == 0) return 0;
            float popCount = PopCount();
            return popCount / len;
        }
        /// <summary>
        /// Returns the number of bits set to 1
        /// </summary>
        /// <returns></returns>
        public int PopCount()
        {
            var len = Length;
            if (len == 0) return 0;
            using var buffer = Buffer;
            if (buffer == null) return 0;
            var bytes = buffer.ReadBytes();
            int setBits = bytes.Sum(o => BitOperations.PopCount(o));
            return setBits;
        }
    }
}
