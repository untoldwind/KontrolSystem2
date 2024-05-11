﻿using System.Linq;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Game.Science;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPScience;

public partial class KSPScienceModule {
    [KSClass("Experiment",
        Description = "Represents an in-game science experiment.")]
    public class ExperimentAdapter(SimulationObjectModel simulationObject, ExperimentStanding experimentStanding, 
        ScienceLocationRegionSituation scienceLocationRegionSituation, ExperimentConfiguration experimentConfiguration) : BaseExperimentAdapter(experimentConfiguration) {

        [KSField] public bool HasEnoughResources => experimentStanding.HasEnoughResources;

        [KSField] public bool CurrentSituationIsValid => experimentStanding.CurrentSituationIsValid;

        [KSField] public double CurrentRunningTime => experimentStanding.CurrentRunningTime;

        [KSField] public ExperimentState CurrentExperimentState => experimentStanding.CurrentExperimentState;

        [KSField] public ExperimentState PreviousExperimentState => experimentStanding.PreviousExperimentState;

        [KSField]
        public ResearchLocationAdapter[] ValidLocations =>
            experimentStanding.ExperimentDefinition.ValidLocations
                .Select(location => new ResearchLocationAdapter(location)).ToArray();

        [KSField] public bool RegionRequired => experimentStanding.RegionRequired;

        [KSField(Description = "Get the research location the experiment was last performed.")]
        public Option<ResearchLocationAdapter> ExperimentLocation =>
            experimentStanding.ExperimentLocation != null
                ? new Option<ResearchLocationAdapter>(
                    new ResearchLocationAdapter(experimentStanding.ExperimentLocation))
                : new Option<ResearchLocationAdapter>();

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

        [KSMethod]
        public double PotentialScienceValue() {
            var value = 0.0;
            var location = scienceLocationRegionSituation.ResearchLocation;

            if (location != null) {
                var scalar = scienceLocationRegionSituation.SituationScalar * scienceLocationRegionSituation.ScienceRegionScalar * scienceLocationRegionSituation.CelestialBodyScalar;
                var definition = experimentStanding.ExperimentDefinition;
                if (definition.ExperimentType == ScienceExperimentType.DataType ||
                    definition.ExperimentType == ScienceExperimentType.Both) {
                    value += KSPContext.CurrentContext.Game.ScienceManager.GetPotentialReportValueScaled(
                        new ResearchReport(definition.ExperimentID, definition.DataReportDisplayName, location,
                            ScienceReportType.DataType, scalar * definition.DataValue, ""));
                }

                if (definition.ExperimentType == ScienceExperimentType.SampleType ||
                    definition.ExperimentType == ScienceExperimentType.Both) {
                    value += KSPContext.CurrentContext.Game.ScienceManager.GetPotentialReportValueScaled(
                        new ResearchReport(definition.ExperimentID, definition.DataReportDisplayName, location,
                            ScienceReportType.SampleType, scalar * definition.SampleValue, ""));
                }
            }

            return value;
        }
    }
}
