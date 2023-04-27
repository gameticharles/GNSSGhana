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
using java.io;

namespace org.gogpsproject.producer
{


	public interface Streamable
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int write(java.io.DataOutputStream dos) throws java.io.IOException;
		int write(ObjectInputStream dos);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void read(java.io.DataInputStream dai, boolean oldVersion) throws java.io.IOException;
		void read(ObjectInputStream dai, bool oldVersion);
	}

	public static class Streamable_Fields
	{
		public const string MESSAGE_OBSERVATIONS = "obs";
		public const string MESSAGE_IONO = "ion";
		public const string MESSAGE_EPHEMERIS = "eph";
		public const string MESSAGE_OBSERVATIONS_SET = "eps";
		public const string MESSAGE_COORDINATES = "coo";
	}

}