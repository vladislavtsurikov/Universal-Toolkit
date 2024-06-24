namespace VladislavTsurikov.ColliderSystem.Runtime
{
    public struct MeshTriangle
    {
        public int Index0 { get; }

        public int Index1 { get; }

        public int Index2 { get; }

        public MeshTriangle(int index0, int index1, int index2)
        {
            Index0 = index0;
            Index1 = index1;
            Index2 = index2;
        }
    }
}