/*
 * Copyright (c) 2011 Eugenio Realini, Mirko Reguzzoni, Cryms sagl - Switzerland. All Rights Reserved.
 *
 * This file is part of goGPS Project (goGPS).
 *
 * goGPS is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3
 * of the License, or (at your option) any later version.
 *
 * goGPS is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with goGPS.  If not, see <http://www.gnu.org/licenses/>.
 *
 */
namespace org.gogpsproject.consumer
{

	using RoverPosition = org.gogpsproject.positioning.RoverPosition;

	/// <summary>
	/// @author Lorenzo Patocchi, cryms.com
	/// 
	/// </summary>
	public interface PositionConsumer
	{

		void addCoordinate(RoverPosition coord);
		void @event(int @event);

	}

	public static class PositionConsumer_Fields
	{
		public const int EVENT_START_OF_TRACK = 0;
		public const int EVENT_END_OF_TRACK = 1;
		public const int EVENT_GOGPS_THREAD_ENDED = 2;
	}

}