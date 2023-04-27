using System;
using System.Collections.Generic;
using System.Threading;
using java.io;
using java.lang;
using java.util;
using net.sf.jni4net;
using net.sf.jni4net.adaptors;

/*
 * Copyright (c) 2010, Eugenio Realini, Mirko Reguzzoni, Cryms sagl - Switzerland. All Rights Reserved.
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
 *
 */
namespace org.gogpsproject
{

	using PositionConsumer = org.gogpsproject.consumer.PositionConsumer;
	using org.gogpsproject.positioning;
	using org.gogpsproject.producer;

	/// <summary>
	/// The Class GoGPS.
	/// 
	/// @author Eugenio Realini, Cryms.com
	/// </summary>
	public class GoGPS : Runnable
	{

	    // Frequency selector
	    /// <summary>
	    /// The Constant FREQ_L1. </summary>
	    public const int FREQ_L1 = ObservationSet.L1;

	    /// <summary>
	    /// The Constant FREQ_L2. </summary>
	    public const int FREQ_L2 = ObservationSet.L2;

	    /// <summary>
	    /// The freq. </summary>
	    private int freq = FREQ_L1;

	    // Double-frequency flag
	    /// <summary>
	    /// The dual freq. </summary>
	    private bool dualFreq = false;

	    // Weighting strategy
	    // 0 = same weight for all observations
	    // 1 = weight based on satellite elevation
	    // 2 = weight based on signal-to-noise ratio
	    // 3 = weight based on combined elevation and signal-to-noise ratio
	    public enum WeightingStrategy
	    {
		    EQUAL,
		    SAT_ELEVATION,
		    SIGNAL_TO_NOISE_RATIO,
		    COMBINED_ELEVATION_SNR
	    }

	    /// <summary>
	    /// The weights. </summary>
	    private WeightingStrategy weights = WeightingStrategy.SAT_ELEVATION;


        public sealed class DynamicModel
        {
            //STATIC = 1
	        public static readonly DynamicModel STATIC = new DynamicModel("STATIC", InnerEnum.STATIC, 1);
            //CONST_SPEED = 2,
	        public static readonly DynamicModel CONST_SPEED = new DynamicModel("CONST_SPEED", InnerEnum.CONST_SPEED, 2);
            //CONST_ACCELERATION = 3
	        public static readonly DynamicModel CONST_ACCELERATION = new DynamicModel("CONST_ACCELERATION", InnerEnum.CONST_ACCELERATION, 3);

            private static readonly IList<DynamicModel> valueList = new List<DynamicModel>();

            static DynamicModel()
            {
                valueList.Add(STATIC);
                valueList.Add(CONST_SPEED);
                valueList.Add(CONST_ACCELERATION);
            }

            public enum InnerEnum
            {
                STATIC=1,
                CONST_SPEED=2,
                CONST_ACCELERATION=3
            }

            private readonly string nameValue;
            private readonly int ordinalValue;
            private readonly InnerEnum innerEnumValue;
            private static int nextOrdinal = 0;

            private int order;

            public int Order
            {
                get
                {
                    return order;
                }
            }

            internal DynamicModel(string name, InnerEnum innerEnum, int order)
            {
                this.order = order;

                nameValue = name;
                ordinalValue = nextOrdinal++;
                innerEnumValue = innerEnum;
            }

            public static IList<DynamicModel> values()
            {
                return valueList;
            }

            public InnerEnum InnerEnumValue()
            {
                return innerEnumValue;
            }

            public int ordinal()
            {
                return ordinalValue;
            }

            public override string ToString()
            {
                return nameValue;
            }

            public static DynamicModel valueOf(string name)
            {
                foreach (DynamicModel enumInstance in values())
                {
                    if (enumInstance.nameValue == name)
                    {
                        return enumInstance;
                    }
                }
                throw new System.ArgumentException(name);
            }
        }

        // Kalman filter parameters

        /// <summary>
        /// The dynamic model. </summary>
        private DynamicModel dynamicModel = DynamicModel.CONST_SPEED;

	    /// <summary>
	    /// The cycle slip threshold. </summary>
	    private double cycleSlipThreshold = 1;

	    public enum CycleSlipDetectionStrategy
	    {
		    APPROX_PSEUDORANGE,
		    DOPPLER_PREDICTED_PHASE_RANGE
	    }

        public enum AmbiguityStrategy
        {
            OBSERV,
            APPROX,
            LS
        }

        /// <summary>
        /// The cycle-slip detection strategy. </summary>
        private CycleSlipDetectionStrategy cycleSlipDetectionStrategy = CycleSlipDetectionStrategy.APPROX_PSEUDORANGE;

	    

        /// <summary>
        /// The ambiguity strategy. </summary>
        private AmbiguityStrategy ambiguityStrategy = AmbiguityStrategy.APPROX;

	    /// <summary>
	    /// The Elevation cutoff. </summary>
	    private double cutoff = 15; // Elevation cutoff

	    public enum RunMode
	    {
		    CODE_STANDALONE,
		    CODE_DOUBLE_DIFF,
		    KALMAN_FILTER_CODE_PHASE_STANDALONE,
		    KALMAN_FILTER_CODE_PHASE_DOUBLE_DIFF,
		    CODE_STANDALONE_SNAPSHOT,
		    CODE_STANDALONE_COARSETIME
	    }

	    private RunMode runMode;

	    private Thread runThread = null;

	    /// <summary>
	    /// The navigation. </summary>
	    private NavigationProducer navigation;

	    /// <summary>
	    /// The rover in. </summary>
	    private ObservationsProducer roverIn;

	    /// <summary>
	    /// The master in. </summary>
	    private ObservationsProducer masterIn;

	    /// <summary>
	    /// The rover calculated position </summary>
	    private readonly RoverPosition roverPos;

	    /// <summary>
	    /// The master position </summary>
	    private readonly MasterPosition masterPos;

	    /// <summary>
	    /// Satellite State Information </summary>
	    private readonly Satellites satellites;

	    /// <summary>
	    /// coarse time error </summary>
	    private long offsetms = 0;

	    private List<PositionConsumer> positionConsumers = new List<PositionConsumer>();

	//	private boolean debug = false;
	    private bool debug = true;

	    private bool useDTM_Renamed = false;

	    /// <summary>
	    /// Use Doppler observations in standalone snapshot case </summary>
	    private bool useDoppler_Renamed = true;

	    public static readonly double MODULO1MS = Constants.SPEED_OF_LIGHT / 1000; // check 1ms bit slip
	    public static readonly double MODULO20MS = Constants.SPEED_OF_LIGHT * 20 / 1000; // check 20ms bit slip

	    /// <summary>
	    /// print additional debug information if the true position is known </summary>
	    public RoverPosition truePos;

	    /// <summary>
	    /// max position update for a valid fix </summary>
	    private long posLimit = 100000; // m

	    /// <summary>
	    /// max height for a valid fix </summary>
	    private long maxHeight = 10000;

	    /// <summary>
	    /// max hdop for a valid fix </summary>
	    private double hdopLimit = 20.0;

	    /// <summary>
	    /// max code residual error to exclude a given range (m) </summary>
	    private double codeResidThreshold = 30;

	    /// <summary>
	    /// max code residual error to exclude a given range (m) </summary>
	    private double phaseResidThreshold = 0.05;

	    private bool searchForOutliers_Renamed = false;

	    /// <summary>
	    /// Instantiates a new GoGPS.
	    /// </summary>
	    /// <param name="navigation"> the navigation </param>
	    /// <param name="roverIn"> the rover in </param>
	    /// <param name="masterIn"> the master in </param>
	    public GoGPS(NavigationProducer navigation, ObservationsProducer roverIn, ObservationsProducer masterIn)
	    {

		    this.navigation = navigation;
		    this.roverIn = roverIn;
		    this.masterIn = masterIn;

		    roverPos = new RoverPosition();
		    masterPos = new MasterPosition();
		    satellites = new Satellites(this);
	    }

	    /// <summary>
	    /// Instantiates a new GoGPS.
	    /// </summary>
	    /// <param name="navigation"> the navigation </param>
	    /// <param name="roverIn"> the rover in </param>
	    public GoGPS(NavigationProducer navigation, ObservationsProducer roverIn) : this(navigation, roverIn, null)
	    {
	    }

	    /// <summary>
	    /// Gets the navigation.
	    /// </summary>
	    /// <returns> the navigation </returns>
	    public virtual NavigationProducer Navigation
	    {
		    get
		    {
			return navigation;
		    }
	    }

	    /// <summary>
	    /// Sets the navigation.
	    /// </summary>
	    /// <param name="navigation"> the navigation to set </param>
	    /// <returns>  </returns>
	    public virtual GoGPS setNavigation(NavigationProducer navigation)
	    {
		    this.navigation = navigation;
		    return this;
	    }



	    public virtual ObservationsProducer RoverIn
	    {
		    get
		    {
			return roverIn;
		    }
	    }

	    public virtual ObservationsProducer MasterIn
	    {
		    get
		    {
			return masterIn;
		    }
	    }

		public virtual RoverPosition getRoverPos()
		{
		    return roverPos;
		}

	    public virtual GoGPS setRoverPos(Coordinates roverPos)
	    {
		    roverPos.cloneInto(this.roverPos);
		    return this;
	    }

	    public virtual MasterPosition getMasterPos()
	    {
		    return masterPos;
	    }

	    public virtual GoGPS setMasterPos(Coordinates masterPos)
	    {
		    masterPos.cloneInto(this.masterPos);
		    return this;
	    }

	    public virtual Satellites Sats
	    {
		    get
		    {
			return satellites;
		    }
	    }

	    /// <summary>
	    /// Gets the freq.
	    /// </summary>
	    /// <returns> the freq </returns>
	    public virtual int Freq
	    {
		    get
		    {
			    return freq;
		    }
	    }

	    /// <summary>
	    /// Sets the freq.
	    /// </summary>
	    /// <param name="freq"> the freq to set </param>
	    /// <returns>  </returns>
	    public virtual GoGPS setFreq(int freq)
	    {
		    this.freq = freq;
		    return this;
	    }

	    /// <summary>
	    /// Checks if is dual freq.
	    /// </summary>
	    /// <returns> the dualFreq </returns>
	    public virtual bool DualFreq
	    {
		    get
		    {
			    return dualFreq;
		    }
	    }

	    /// <summary>
	    /// Sets the dual freq.
	    /// </summary>
	    /// <param name="dualFreq"> the dualFreq to set </param>
	    /// <returns>  </returns>
	    public virtual GoGPS setDualFreq(bool dualFreq)
	    {
		    this.dualFreq = dualFreq;
		    return this;
	    }

	    /// <summary>
	    /// Gets the cutoff.
	    /// </summary>
	    /// <returns> the cutoff </returns>
	    public virtual double Cutoff
	    {
		    get
		    {
			    return cutoff;
		    }
	    }

	    /// <summary>
	    /// Sets the cutoff.
	    /// </summary>
	    /// <param name="cutoff"> the cutoff to set </param>
	    public virtual GoGPS setCutoff(double cutoff)
	    {
		    this.cutoff = cutoff;
		    return this;
	    }

	    /// <summary>
	    /// Gets the cycle slip threshold.
	    /// </summary>
	    /// <returns> the cycle slip threshold </returns>
	    public virtual double CycleSlipThreshold
	    {
		    get
		    {
			    return cycleSlipThreshold;
		    }
	    }

	    /// <summary>
	    /// Sets the cycle slip threshold.
	    /// </summary>
	    /// <param name="csThreshold"> the cycle slip threshold to set </param>
	    /// <returns>  </returns>
	    public virtual GoGPS setCycleSlipThreshold(double csThreshold)
	    {
		    this.cycleSlipThreshold = csThreshold;
		    return this;
	    }

	    /// <summary>
	    /// Gets the weights.
	    /// </summary>
	    /// <returns> the weights </returns>
	    public virtual WeightingStrategy Weights
	    {
		    get
		    {
			    return weights;
		    }
            set
            {
                this.Weights = value;
            }
	    }

	    /// <summary>
	    /// Sets the weights.
	    /// </summary>
	    /// <param name="weights"> the weights to set </param>
	    /// <returns>  </returns>
	    public virtual GoGPS setWeights(WeightingStrategy weights)
	    {
		    this.weights = weights;
		    return this;
	    }

	    /// <summary>
	    /// Gets the dynamic model.
	    /// </summary>
	    /// <returns> the dynamicModel </returns>
	    public virtual DynamicModel DynamicModel
	    {
		    get
		    {
			    return dynamicModel;
		    }
	    }

	    /// <summary>
	    /// Sets the dynamic model.
	    /// </summary>
	    /// <param name="dynamicModel"> the dynamicModel to set </param>
	    public virtual GoGPS setDynamicModel(DynamicModel dynamicModel)
	    {
		    this.dynamicModel = dynamicModel;
		    return this;
	    }

	    /// <summary>
	    /// Gets the cycle-slip detection strategy.
	    /// </summary>
	    /// <returns> the cycleSlipDetectionStrategy </returns>
	    public virtual CycleSlipDetectionStrategy CycleSlipDetectionStrategy
	    {
		    get
		    {
			    return cycleSlipDetectionStrategy;
		    }
	    }

	    /// <summary>
	    /// Sets the cycle-slip detection strategy.
	    /// </summary>
	    /// <param name="cycleSlipDetectionStrategy"> the cycleSlipDetectionStrategy to set </param>
	    /// <returns>  </returns>
	    public virtual GoGPS setCycleSlipDetection(CycleSlipDetectionStrategy cycleSlipDetectionStrategy)
	    {
		    this.cycleSlipDetectionStrategy = cycleSlipDetectionStrategy;
		    return this;
	    }

	    /// <summary>
	    /// Gets the ambiguity strategy.
	    /// </summary>
	    /// <returns> the ambiguityStrategy </returns>
	    public virtual AmbiguityStrategy AmbiguityStrategy
	    {
		    get
		    {
			    return ambiguityStrategy;
		    }
	    }

	    /// <summary>
	    /// Sets the ambiguity strategy.
	    /// </summary>
	    /// <param name="ambiguityStrategy"> the ambiguityStrategy to set </param>
	    /// <returns>  </returns>
	    public virtual GoGPS setAmbiguityStrategy(AmbiguityStrategy ambiguityStrategy)
	    {
		    this.ambiguityStrategy = ambiguityStrategy;
		    return this;
	    }

	    /// <returns> the debug </returns>
	    public virtual bool Debug
	    {
		    get
		    {
			    return debug;
		    }
	    }

	    /// <param name="debug"> the debug to set </param>
	    /// <returns>  </returns>
	    public virtual GoGPS setDebug(bool debug)
	    {
		    this.debug = debug;
		    return this;
	    }

	    public virtual bool useDTM()
	    {
		    return useDTM_Renamed;
	    }

	    public virtual GoGPS useDTM(bool useDTM)
	    {
		    this.useDTM_Renamed = useDTM;
		    return this;
	    }

	    public virtual long MaxHeight
	    {
		    get
		    {
			return maxHeight;
		    }
	    }

	    public virtual GoGPS setMaxHeight(long maxHeight)
	    {
		    this.maxHeight = maxHeight;
		    return this;
	    }

	    public virtual double CodeResidThreshold
	    {
		    get
		    {
			return this.codeResidThreshold;
		    }
	    }

	    public virtual GoGPS setCodeResidThreshold(double codeResidThreshold)
	    {
		    this.codeResidThreshold = codeResidThreshold;
		    return this;
	    }

	    public virtual double PhaseResidThreshold
	    {
		    get
		    {
			return this.phaseResidThreshold;
		    }
	    }

	    public virtual GoGPS setPhaseResidThreshold(double phaseResidThreshold)
	    {
		    this.phaseResidThreshold = phaseResidThreshold;
		    return this;
	    }

	    public virtual GoGPS searchForOutliers(bool searchForOutliers)
	    {
		    this.searchForOutliers_Renamed = searchForOutliers;
		    return this;
	    }

	    public virtual bool searchForOutliers()
	    {
		    return searchForOutliers_Renamed;
	    }

	    public virtual double HdopLimit
	    {
		    get
		    {
			return hdopLimit;
		    }
	    }

	    public virtual GoGPS setHdopLimit(double hdopLimit)
	    {
		    this.hdopLimit = hdopLimit;
		    return this;
	    }

	    public virtual long PosLimit
	    {
		    get
		    {
			return posLimit;
		    }
	    }

	    public virtual GoGPS setPosLimit(long maxPosUpdate)
	    {
		    this.posLimit = maxPosUpdate;
		    return this;
	    }

	    public virtual long Offsetms
	    {
		    get
		    {
			return offsetms;
		    }
	    }

	    public virtual GoGPS setOffsetms(long offsetms)
	    {
		    this.offsetms = offsetms;
		    return this;
	    }

	    /// <summary>
	    /// Use Doppler observations in standalone snapshot case </summary>
	    public virtual bool useDoppler()
	    {
		    return useDoppler_Renamed;
	    }

	    /// <summary>
	    /// Use Doppler observations in standalone snapshot case </summary>
	    public virtual GoGPS useDoppler(bool useDoppler)
	    {
		    this.useDoppler_Renamed = useDoppler;
		    return this;
	    }

		/// <returns> the positionConsumer </returns>
		public virtual void cleanPositionConsumers()
		{
			positionConsumers.Clear();
		}

		public virtual void removePositionConsumer(PositionConsumer positionConsumer)
		{
			positionConsumers.Remove(positionConsumer);
		}

		public virtual List<PositionConsumer> PositionConsumers
		{
			get
			{
			return positionConsumers;
			}
		}

	    /// <param name="positionConsumer"> the positionConsumer to add </param>
		public virtual GoGPS addPositionConsumerListener(PositionConsumer positionConsumer)
		{
			this.positionConsumers.Add(positionConsumer);
			return this;
		}

	    public virtual GoGPS addPositionConsumerListeners(params PositionConsumer[] positionConsumers)
	    {
		    this.positionConsumers.AddRange(positionConsumers);
		    return this;
	    }

		public virtual void notifyPositionConsumerEvent(int @event)
		{
			foreach (PositionConsumer pc in positionConsumers)
			{
				try
				{
					pc.@event(@event);
				}
				catch (System.Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}

		public virtual void notifyPositionConsumerAddCoordinate(RoverPosition coord)
		{
			foreach (PositionConsumer pc in positionConsumers)
			{
				try
				{
					pc.addCoordinate(coord);
				}
				catch (System.Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}
        
		/// <returns> the runMode </returns>
		public virtual RunMode RunMode
		{
			get
            {
                return runMode;
            }
		}

		/// <param name="runMode"> the run mode to use </param>
		public virtual GoGPS runThreadMode(RunMode runMode)
		{
			return run(runMode, true);
		}

	    public virtual GoGPS run(RunMode runMode)
	    {
		    return run(runMode, false);
	    }

	    /// <param name="runMode"> the run mode to use </param>
	    public virtual GoGPS run(RunMode runMode, bool threadMode)
	    {
		    this.runMode = runMode;

		    if (threadMode)
		    {
		        runThread = new Thread(this);

		        switch (runMode)
		        {
			        case RunMode.CODE_STANDALONE:
			            runThread.Name = "goGPS standalone";
			            break;
			        case RunMode.CODE_DOUBLE_DIFF:
			            runThread.Name = "goGPS double difference";
			            break;
			        case RunMode.KALMAN_FILTER_CODE_PHASE_STANDALONE:
			            runThread.Name = "goGPS Kalman filter standalone";
			            break;
			        case RunMode.KALMAN_FILTER_CODE_PHASE_DOUBLE_DIFF:
			            runThread.Name = "goGPS Kalman filter double difference";
			            break;
			        case RunMode.CODE_STANDALONE_SNAPSHOT:
			            runThread.Name = "goGPS standalone snapshot";
			            break;
			        case RunMode.CODE_STANDALONE_COARSETIME:
			            runThread.Name = "goGPS standalone coarse time";
			            break;
		        }

		        runThread.Start();
		    }
		    else
		    {
		        run();
		    }

		    return this;
	    }

	    /* (non-Javadoc)
	    * @see java.lang.Runnable#run()
	    */
	    public void run()
	    {
		    if (runMode == null)
		    {
			    throw new System.Exception("runMode was not defined. Ex. goGPS.run( RunMode.CODE_STANDALONE )");
		    }

		    switch (runMode)
		    {
			    case RunMode.CODE_STANDALONE:
			        LS_SA_code.run(this, -1);
				    break;
			    case RunMode.CODE_DOUBLE_DIFF:
				    LS_DD_code.run(this);
				    break;
			    case RunMode.KALMAN_FILTER_CODE_PHASE_STANDALONE:
				    KF_SA_code_phase.run(this);
				    break;
			    case RunMode.KALMAN_FILTER_CODE_PHASE_DOUBLE_DIFF:
				    KF_DD_code_phase.run(this);
				    break;
		        case RunMode.CODE_STANDALONE_SNAPSHOT:
			        LS_SA_code_snapshot.run(this);
			        break;
		        case RunMode.CODE_STANDALONE_COARSETIME:
			        LS_SA_code_coarse_time.run(this, MODULO20MS);
			        break;
		    }

		    notifyPositionConsumerEvent(consumer.PositionConsumer_Fields.EVENT_GOGPS_THREAD_ENDED);
	    }

	    /// <summary>
	    /// Run code standalone.
	    /// </summary>
	    /// <param name="getNthPosition"> the get nth position </param>
	    /// <returns> the coordinates </returns>
	    /// <exception cref="Exception"> </exception>
	    public virtual GoGPS runCodeStandalone(double stopAtDopThreshold)
	    {
		    LS_SA_code.run(this, stopAtDopThreshold);
		    return this;
	    }

	    public virtual bool Running
	    {
		    get
		    {
			return runThread != null && runThread.IsAlive;
		    }
	    }

	    public virtual string ThreadName
	    {
		    get
		    {
			    if (runThread != null)
			    {
			        return runThread.Name;
			    }
			    else
			    {
			        return "";
			    }
		    }
	    }

	    public virtual void stopThreadMode()
	    {
		    if (Running)
		    {
		        try
		        {
			        runThread.Interrupt();
			        runThread.Join();
		        }
		        catch (System.Exception e)
		        {
		        }
		    }
	    }

		public virtual GoGPS runUntilFinished()
		{
		    foreach (PositionConsumer pc in positionConsumers)
		    {
		        if (pc is Thread)
		        {
			        try
			        {
			            ((Thread)pc).Join();
			        }
			        catch (System.Exception)
			        {
			        }
		        }
		    }

		    return this;
		}

	    public virtual GoGPS runFor(int seconds)
	    {
		    try
		    {
		        Thread.Sleep(seconds * 1000);
		    }
		    catch (System.Exception)
		    {
		    }

		    return this;
	    }

	}

}