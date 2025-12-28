using Audio;

namespace Databases
{
    public class WorldAudioDatabase : DatabaseBase<WorldAudioData>
    {
        protected override string GetKey(WorldAudioData entry) => entry.ID;
    }
}