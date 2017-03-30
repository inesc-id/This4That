#region Copyright (c) 2009-2016 Misakai Ltd.
/*************************************************************************
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as
* published by the Free Software Foundation, either version 3 of the
* License, or(at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with this program.If not, see<http://www.gnu.org/licenses/>.
*************************************************************************/
#endregion Copyright (c) 2009-2016 Misakai Ltd.

namespace Emitter
{
    /// <summary>
    /// Contals all of the constants we use in this project.
    /// </summary>
    internal static class EmitterConst
    {
        // Separator for the channel (eg: forex/+/usd/)
        public const char Separator = '/';

        // The maximum frame size
        public const int FrameSize = 1 * 1024 * 1024; // 1MB

        // Various sizes of the packet elements
        public const int DefaultTTL = 3600;

        // The prefix for the API methods
        public const string ApiPrefix = "emitter";

        // Transient TTL value
        public const int Transient = 0;
    }
}