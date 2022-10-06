using System;

namespace CodeBase.Data
{
    [Serializable]

    public class VectorPositionOnLevel
    {
        public string Level; 
        public Vector3Data Position;

        public VectorPositionOnLevel(string level, Vector3Data position) {
            Level = level;
            Position = position;
        }
    }
}