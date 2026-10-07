using System;
using System.Collections.Generic;

namespace Game.Core.Save
{
    public static class SaveMigrations
    {
        public static IReadOnlyList<ISaveMigration> All { get; } = Array.Empty<ISaveMigration>();
    }
}
