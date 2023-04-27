using System;

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
namespace org.gogpsproject.producer.parser.nvs
{
	/// <summary>
	/// <para>
	/// 
	/// </para>
	/// 
	/// @author Daisuke Yoshida OCU
	/// </summary>

	/// <summary>
	/// @author Yoshida
	/// 
	/// </summary>
	public class NVSException : Exception
	{


		/// 
		private const long serialVersionUID = -9199014444784010570L;

		/// <param name="message"> </param>
		public NVSException(string message) : base(message)
		{
		}

		/// <param name="cause"> </param>
		public NVSException(Exception cause) : base(cause)
		{
		}

		/// <param name="message"> </param>
		/// <param name="cause"> </param>
		public NVSException(string message, Exception cause) : base(message, cause)
		{
		}

	}

}