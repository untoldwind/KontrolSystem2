﻿using KontrolSystem.TO2.Binding;
using KSP.Game.Science;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPScience {
    public partial class KSPScienceModule {
        [KSClass("Experiment",
            Description = "Represents an in-game science experiment.")]
        public class ExperimentAdapter {
            private readonly SimulationObjectModel simulationObject;
            private readonly ExperimentStanding experimentStanding;
            private readonly ExperimentConfiguration experimentConfiguration;

            public ExperimentAdapter(SimulationObjectModel simulationObject, ExperimentStanding experimentStanding, ExperimentConfiguration experimentConfiguration) {
                this.simulationObject = simulationObject;
                this.experimentStanding = experimentStanding;
                this.experimentConfiguration = experimentConfiguration;
            }

            [KSField]
            public ExperimentDefinitionAdapter Definition =>
                new ExperimentDefinitionAdapter(experimentStanding.ExperimentDefinition);

            [KSField] public long CrewRequired => experimentConfiguration.CrewRequired;

            [KSField] public double TimeToComplete => experimentConfiguration.TimeToComplete;

            [KSField] public bool HasEnoughResources => experimentStanding.HasEnoughResources;

            [KSField] public bool CurrentSituationIsValid => experimentStanding.CurrentSituationIsValid;

            [KSField] public double CurrentRunningTime => experimentStanding.CurrentRunningTime;

            [KSField] public bool ConditionMet => experimentStanding.ConditionMet;

            [KSField] public ExperimentState CurrentExperimentState => experimentStanding.CurrentExperimentState;

            [KSField] public ExperimentState PreviousExperimentState => experimentStanding.PreviousExperimentState;

            [KSMethod] 
            public bool PauseExperiment() {
                if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(simulationObject,
                        out var viewObject)) return false;

                if (!viewObject.TryGetComponent<Module_ScienceExperiment>(out var moduleScienceExperiment)) return false;

                moduleScienceExperiment.OnPauseExperiment(experimentStanding.ExperimentID);
                return true;
            }

            [KSMethod] 
            public bool CancelExperiment() {
                if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(simulationObject,
                        out var viewObject)) return false;

                if (!viewObject.TryGetComponent<Module_ScienceExperiment>(out var moduleScienceExperiment)) return false;

                moduleScienceExperiment.OnCancelExperiment(experimentStanding.ExperimentID);
                return true;
            }

            [KSMethod]
            public bool RunExperiment() {
                if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(simulationObject,
                        out var viewObject)) return false;

                if (!viewObject.TryGetComponent<Module_ScienceExperiment>(out var moduleScienceExperiment)) return false;
                
                moduleScienceExperiment.OnAttemptToRunExperiment(experimentStanding.ExperimentID);
                return true;
            }
        }
    }
}