/*
 * Copyright (c) 2010 Eugenio Realini, Mirko Reguzzoni, Cryms sagl - Switzerland. All Rights Reserved.
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
namespace org.gogpsproject.producer
{

	using EphGps = org.gogpsproject.ephemeris.EphGps;
	using Coordinates = org.gogpsproject.positioning.Coordinates;
	using IonoGps = org.gogpsproject.producer.parser.IonoGps;

	/// <summary>
	/// @author Cryms.com
	/// 
	/// </summary>
	public interface StreamEventListener
	{

		void streamClosed();
		void addObservations(Observations o);
		void addIonospheric(IonoGps iono);
		void addEphemeris(EphGps eph);

		Coordinates DefinedPosition {set;}

		Observations CurrentObservations {get;}
		void pointToNextObservations();
	}

}