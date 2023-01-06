using SeaPublicWebsite.BusinessLogic.ExternalServices.Bre.Enums;
using SeaPublicWebsite.BusinessLogic.Models;
using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace SeaPublicWebsite.BusinessLogic.ExternalServices.Bre
{
    public class RecommendationService
    {
        private readonly BreApi breApi;

        public RecommendationService(BreApi breApi)
        {
            this.breApi = breApi;
        }

        public static readonly Dictionary<string, BreRecommendation> RecommendationDictionary =
            new()
            {
                {
                    "A", new BreRecommendation
                    {
                        Key = RecommendationKey.AddLoftInsulation,
                        Title = "Add some more loft insulation",
                        Summary = "Increase the level of insulation in your loft to the recommended level of 300mm"
                    }
                },
                {
                    "A1", new BreRecommendation
                    {
                        Key = RecommendationKey.InsulateYourLoft,
                        Title = "Insulate your loft",
                        Summary = "Add loft insulation to reach the recommended level of 300mm"
                    }
                },
                {
                    "B", new BreRecommendation
                    {
                        Key = RecommendationKey.InsulateCavityWalls,
                        Title = "Insulate your cavity walls",
                        Summary = "Inject insulation into the cavity in your external walls"
                    }
                },
                {
                    "Q", new BreRecommendation
                    {
                        Key = RecommendationKey.InsulateSolidWalls,
                        Title = "Insulate your solid walls",
                        Summary = "Insulate your solid walls (brick walls age A-D)"
                    }
                },
                {
                    "Q1", new BreRecommendation
                    {
                        Key = RecommendationKey.InsulateSolidWalls,
                        Title = "Insulate your solid walls",
                        Summary = "Insulate your solid walls (other wall types)"
                    }
                },
                {
                    "W1", new BreRecommendation
                    {
                        Key = RecommendationKey.FloorInsulationSuspendedFloor,
                        Title = "Insulate the ground floor",
                        Summary = "Insulate the ground floor"
                    }
                },
                {
                    "C", new BreRecommendation
                    {
                        Key = RecommendationKey.HotWaterCylinderInsulation,
                        Title = "Insulate your hot water cylinder",
                        Summary = "Insulate your hot water cylinder"
                    }
                },
                {
                    "G", new BreRecommendation
                    {
                        Key = RecommendationKey.UpgradeHeatingControls,
                        Title = "Upgrade your heating controls",
                        Summary = "Fit a programmer, thermostat and thermostatic radiator valves"
                    }
                },
                {
                    "L2", new BreRecommendation
                    {
                        Key = RecommendationKey.HighHeatRetentionStorageHeaters,
                        Title = "Upgrade to high heat retention storage heaters",
                        Summary = "Upgrade to high heat retention storage heaters"
                    }
                },
                {
                    "O", new BreRecommendation
                    {
                        Key = RecommendationKey.ReplaceSingleGlazedWindowsWithDoubleOrTripleGlazing,
                        Title = "Fit new windows",
                        Summary = "Replace old single glazed windows with new double or triple glazing"
                    }
                },
                {
                    "U", new BreRecommendation
                    {
                        Key = RecommendationKey.SolarElectricPanels,
                        Title = "Fit solar electric panels",
                        Summary = "Install PV panels on your roof to generate electricity"
                    }
                },
                {
                    "Z1", new BreRecommendation
                    {
                        Key = RecommendationKey.InstallHeatPump,
                        Title = "Install a heat pump",
                        Summary = "Heat pumps use the energy outside to heat your home and hot water"
                    }
                }
            };

        public async Task<List<BreRecommendation>> GetRecommendationsForPropertyAsync(PropertyData propertyData)
        {
            BreRequest request = CreateRequest(propertyData);

            return await breApi.GetRecommendationsForPropertyRequestAsync(request);
        }

        private static BreRequest CreateRequest(PropertyData propertyData)
        {
            BrePropertyType brePropertyType = GetBrePropertyType(propertyData.PropertyType.Value);

            BreBuiltForm breBuiltForm =
                GetBreBuiltForm(propertyData.PropertyType.Value, propertyData.HouseType, propertyData.BungalowType);

            BreFlatLevel? breFlatLevel = GetBreFlatLevel(propertyData.PropertyType.Value, propertyData.FlatType);

            string breConstructionDate = GetBreConstructionDate(propertyData.YearBuilt, propertyData.WallConstruction, propertyData.CavityWallsInsulated, propertyData.Epc?.ConstructionAgeBand);

            BreWallType breWallType = GetBreWallType(propertyData.WallConstruction.Value,
                propertyData.SolidWallsInsulated,
                propertyData.CavityWallsInsulated);

            BreRoofType? breRoofType = GetBreRoofType(propertyData.RoofConstruction, propertyData.LoftSpace, 
                propertyData.LoftAccess, propertyData.RoofInsulated);

            BreGlazingType breGlazingType = GetBreGlazingType(propertyData.GlazingType.Value);

            BreHeatingSystem breHeatingSystem =
                GetBreHeatingSystem(propertyData.HeatingType.Value, propertyData.OtherHeatingType);

            bool? breHotWaterCylinder = GetBreHotWaterCylinder(propertyData.HasHotWaterCylinder);

            BreHeatingPatternType breHeatingPatternType = GetBreHeatingPatternType(propertyData.HeatingPattern.Value,
                propertyData.HoursOfHeatingEvening, propertyData.HoursOfHeatingEvening);

            int[] breNormalDaysOffHours =
                GetBreNormalDaysOffHours(propertyData.HoursOfHeatingMorning, propertyData.HoursOfHeatingEvening);

            BreFloorType? breFloorType = GetBreFloorType(propertyData.FloorConstruction, propertyData.FloorInsulated);

            bool? breOutsideSpace = GetBreOutsideSpace(propertyData.HasOutdoorSpace);

            BreRequest request = new(
                brePropertyType: brePropertyType,
                breBuiltForm: breBuiltForm,
                breFlatLevel: breFlatLevel,
                breConstructionDate: breConstructionDate,
                breWallType: breWallType,
                breRoofType: breRoofType,
                breGlazingType: breGlazingType,
                breHeatingSystem: breHeatingSystem,
                breHotWaterCylinder: breHotWaterCylinder,
                breOccupants: propertyData.NumberOfOccupants,
                breHeatingPatternType: breHeatingPatternType,
                breNormalDaysOffHours: breNormalDaysOffHours,
                breTemperature: propertyData.Temperature,
                breFloorType: breFloorType,
                breOutsideSpace: breOutsideSpace
            );

            return request;
        }

        private static BrePropertyType GetBrePropertyType(PropertyType propertyType)
        {
            return propertyType switch
            {
                PropertyType.House => BrePropertyType.House,
                PropertyType.Bungalow => BrePropertyType.Bungalow,
                // peer-reviewed assumption:
                PropertyType.ApartmentFlatOrMaisonette => BrePropertyType.Flat,
                _ => throw new ArgumentNullException()
            };
        }

        private static BreBuiltForm GetBreBuiltForm(PropertyType propertyType, HouseType? houseType,
            BungalowType? bungalowType)
        {
            return propertyType switch
            {
                PropertyType.House => houseType switch
                {
                    HouseType.Detached => BreBuiltForm.Detached,
                    HouseType.SemiDetached => BreBuiltForm.SemiDetached,
                    //peer-reviewed assumption:
                    HouseType.EndTerrace => BreBuiltForm.EndTerrace,
                    //peer-reviewed assumption:
                    HouseType.Terraced => BreBuiltForm.MidTerrace,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PropertyType.Bungalow => bungalowType switch
                {
                    BungalowType.Detached => BreBuiltForm.Detached,
                    BungalowType.SemiDetached => BreBuiltForm.SemiDetached,
                    //peer-reviewed assumption:
                    BungalowType.EndTerrace => BreBuiltForm.EndTerrace,
                    //peer-reviewed assumption:
                    BungalowType.Terraced => BreBuiltForm.MidTerrace,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PropertyType.ApartmentFlatOrMaisonette =>
                    //the BreBuiltForm values don't make sense for flats, but built_form is a required input to the
                    //BRE API even when property_type is Flat  so we set EnclosedEndTerrace as a default value
                    //(this value indicates two adjacent exposed walls which seems a good average for a flat):
                    BreBuiltForm.EnclosedEndTerrace,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static BreFlatLevel? GetBreFlatLevel(PropertyType propertyType, FlatType? flatType)
        {
            switch (propertyType)
            {
                case PropertyType.ApartmentFlatOrMaisonette:
                    return flatType switch
                    {
                        FlatType.TopFloor => BreFlatLevel.TopFloor,
                        FlatType.MiddleFloor => BreFlatLevel.MidFloor,
                        FlatType.GroundFloor => BreFlatLevel.GroundFloor,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                default:
                    return null;
            }
        }

        private static string GetBreConstructionDate(YearBuilt? yearBuilt, WallConstruction? wallConstruction, CavityWallsInsulated? cavityWallsInsulated,  HomeAge? epcConstructionAgeBand)
        {
            return yearBuilt switch
            {
                YearBuilt.Pre1930 => "B",
                YearBuilt.From1930To1966 => "D",
                YearBuilt.From1967To1982 => "F",
                YearBuilt.From1983To1995 => "H",
                YearBuilt.From1996To2011 => "K",
                YearBuilt.From2012ToPresent => "L",
                //peer-reviewed assumptions:
                _ => epcConstructionAgeBand switch
                {
                    HomeAge.Pre1900 => "A",
                    HomeAge.From1900To1929 => "B",
                    HomeAge.From1930To1949 => "C",
                    HomeAge.From1950To1966 => "D",
                    HomeAge.From1967To1975 => "E",
                    HomeAge.From1976To1982 => "F",
                    HomeAge.From1983To1990 => "G",
                    HomeAge.From1991To1995 => "H",
                    HomeAge.From1996To2002 => "I",
                    HomeAge.From2003To2006 => "J",
                    HomeAge.From2007To2011 => "K",
                    HomeAge.From2012ToPresent => "L",
                    _ => wallConstruction switch
                    {
                        WallConstruction.DoNotKnow => "D",
                        WallConstruction.Solid => "B",
                        WallConstruction.Cavity => cavityWallsInsulated switch
                        {
                            CavityWallsInsulated.DoNotKnow => "D",
                            CavityWallsInsulated.No => "D",
                            CavityWallsInsulated.Some => "D",
                            CavityWallsInsulated.All => "I",
                            _ => throw new ArgumentOutOfRangeException()
                        },
                        WallConstruction.Mixed => "B",
                        WallConstruction.Other => "D",
                        _ => throw new ArgumentOutOfRangeException()
                    },
                },
            };
        }

        private static BreWallType GetBreWallType(WallConstruction wallConstruction,
            SolidWallsInsulated? solidWallsInsulated,
            CavityWallsInsulated? cavityWallsInsulated)
        {
            return wallConstruction switch
            {
                WallConstruction.DoNotKnow => BreWallType.DontKnow,
                WallConstruction.Solid => solidWallsInsulated switch
                {
                    SolidWallsInsulated.DoNotKnow => BreWallType.DontKnow,
                    SolidWallsInsulated.No => BreWallType.SolidWallsWithoutInsulation,
                    //peer-reviewed assumption:
                    SolidWallsInsulated.Some => BreWallType.SolidWallsWithoutInsulation,
                    SolidWallsInsulated.All => BreWallType.SolidWallsWithInsulation,
                    _ => throw new ArgumentOutOfRangeException()
                },
                WallConstruction.Cavity => cavityWallsInsulated switch
                {
                    CavityWallsInsulated.DoNotKnow => BreWallType.DontKnow,
                    CavityWallsInsulated.No => BreWallType.CavityWallsWithoutInsulation,
                    //peer-reviewed assumption:
                    CavityWallsInsulated.Some => BreWallType.CavityWallsWithoutInsulation,
                    CavityWallsInsulated.All => BreWallType.CavityWallsWithInsulation,
                    _ => throw new ArgumentOutOfRangeException()
                },
                WallConstruction.Mixed => cavityWallsInsulated switch
                {
                    CavityWallsInsulated.DoNotKnow => solidWallsInsulated switch
                    {
                        //peer-reviewed assumption:
                        SolidWallsInsulated.DoNotKnow => BreWallType.DontKnow,
                        //peer-reviewed assumption (may change to DontKnow if this can drive recommendations):
                        SolidWallsInsulated.No => BreWallType.SolidWallsWithoutInsulation,
                        //peer-reviewed assumption (may change to DontKnow if this can drive recommendations):
                        SolidWallsInsulated.Some => BreWallType.SolidWallsWithoutInsulation,
                        //peer-reviewed assumption:
                        SolidWallsInsulated.All => BreWallType.DontKnow,
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    //peer-reviewed assumption:
                    CavityWallsInsulated.No => BreWallType.CavityWallsWithoutInsulation,
                    //peer-reviewed assumption:
                    CavityWallsInsulated.Some => BreWallType.CavityWallsWithoutInsulation,
                    CavityWallsInsulated.All => solidWallsInsulated switch
                    {
                        //peer-reviewed assumption (may change to DontKnow if this can drive recommendations):
                        SolidWallsInsulated.DoNotKnow => BreWallType.SolidWallsWithoutInsulation,
                        //peer-reviewed assumption:
                        SolidWallsInsulated.No => BreWallType.SolidWallsWithoutInsulation,
                        //peer-reviewed assumption:
                        SolidWallsInsulated.Some => BreWallType.SolidWallsWithoutInsulation,
                        //peer-reviewed assumption:
                        SolidWallsInsulated.All => BreWallType.SolidWallsWithInsulation,
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    _ => throw new ArgumentOutOfRangeException()
                },
                WallConstruction.Other => BreWallType.OtherWallType,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static BreRoofType? GetBreRoofType(RoofConstruction? roofConstruction,
            LoftSpace? loftSpace, LoftAccess? loftAccess, RoofInsulated? roofInsulated)
        {
            return roofConstruction switch
            {
                RoofConstruction.Flat =>
                    //peer-reviewed assumption:
                    BreRoofType.FlatRoofWithInsulation,
                RoofConstruction.Pitched or RoofConstruction.Mixed => loftSpace switch
                {
                    //peer-reviewed assumption:
                    LoftSpace.No => BreRoofType.PitchedRoofWithInsulation,
                    LoftSpace.Yes => loftAccess switch
                    {
                        LoftAccess.No => BreRoofType.DontKnow,
                        LoftAccess.Yes => roofInsulated switch
                        {
                            RoofInsulated.DoNotKnow => BreRoofType.DontKnow,
                            //peer-reviewed assumption in case RoofConstruction.Mixed:
                            RoofInsulated.Yes => BreRoofType.PitchedRoofWithInsulation,
                            //peer-reviewed assumption in case RoofConstruction.Mixed:
                            RoofInsulated.No => BreRoofType.PitchedRoofWithoutInsulation,
                            _ => throw new ArgumentOutOfRangeException()
                        },
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    _ => throw new ArgumentOutOfRangeException()
                },
                //peer-reviewed assumption for ground and middle floor flats
                _ => null
            };
        }

        private static BreGlazingType GetBreGlazingType(GlazingType glazingType)
        {
            return glazingType switch
            {
                //peer-reviewed assumption (BreGlazingType.DontKnow would return recommendation O3 rather than O, which we don't want):
                GlazingType.DoNotKnow => BreGlazingType.SingleGlazed,
                GlazingType.SingleGlazed => BreGlazingType.SingleGlazed,
                //peer-reviewed assumption, this will return recommendation O3, currently not whitelisted in BreRequest.cs:
                GlazingType.DoubleOrTripleGlazed => BreGlazingType.DoubleGlazed,
                //peer-reviewed assumption:
                GlazingType.Both => BreGlazingType.SingleGlazed,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static BreHeatingSystem GetBreHeatingSystem(HeatingType heatingType, OtherHeatingType? otherHeatingType)
        {
            return heatingType switch
            {
                HeatingType.DoNotKnow => BreHeatingSystem.GasBoiler,
                HeatingType.GasBoiler => BreHeatingSystem.GasBoiler,
                HeatingType.OilBoiler => BreHeatingSystem.OilBoiler,
                HeatingType.LpgBoiler => BreHeatingSystem.LpgBoiler,
                HeatingType.Storage => BreHeatingSystem.StorageHeaters,
                HeatingType.DirectActionElectric => BreHeatingSystem.DirectActingElectric,
                HeatingType.HeatPump => BreHeatingSystem.HeatPump,
                HeatingType.Other => otherHeatingType switch
                {
                    OtherHeatingType.Biomass => BreHeatingSystem.BiomassBoiler,
                    OtherHeatingType.CoalOrSolidFuel => BreHeatingSystem.SolidFuelBoiler,
                    //peer-reviewed assumption:
                    OtherHeatingType.Other => BreHeatingSystem.GasBoiler,
                    _ => throw new ArgumentOutOfRangeException()
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static bool? GetBreHotWaterCylinder(HasHotWaterCylinder? hasHotWaterCylinder)
        {
            return hasHotWaterCylinder switch
            {
                HasHotWaterCylinder.DoNotKnow => null,
                HasHotWaterCylinder.Yes => true,
                HasHotWaterCylinder.No => false,
                _ => null
            };
        }

        private static BreHeatingPatternType GetBreHeatingPatternType(HeatingPattern? heatingPattern,
            int? hoursOfHeatingMorning, int? hoursOfHeatingEvening)
        {
            if (hoursOfHeatingMorning == 0 && hoursOfHeatingEvening == 0)
            {
                //User has input values that correspond to a pre-existing BreHeatingPatternType
                return BreHeatingPatternType.NoneOfTheAbove;
            }

            if (hoursOfHeatingMorning == 12 && hoursOfHeatingEvening == 12)
            {
                //User has input values that correspond to a pre-existing BreHeatingPatternType
                return BreHeatingPatternType.AllDayAndAllNight;
            }

            return heatingPattern switch
            {
                HeatingPattern.AllDayAndNight => BreHeatingPatternType.AllDayAndAllNight,
                HeatingPattern.AllDayNotNight => BreHeatingPatternType.AllDayButOffAtNight,
                HeatingPattern.Other => BreHeatingPatternType.NoneOfTheAbove,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static int[] GetBreNormalDaysOffHours(int? hoursOfHeatingMorning, int? hoursOfHeatingEvening)
        {
            if (hoursOfHeatingMorning != null && hoursOfHeatingEvening != null)
            {
                //This condition has the secondary purpose of guaranteeing that the sum of the two off periods returned
                //later is no more than 23 as is required by BRE 
                if ((hoursOfHeatingMorning == 0 && hoursOfHeatingEvening == 0)
                    || (hoursOfHeatingMorning == 12 && hoursOfHeatingEvening == 12))
                {
                    //User has input values that correspond to a pre-existing BreHeatingPatternType
                    return null;
                }

                //Peer-reviewed assumptions: time heating is turned on is not collected so we assume morning heating
                //starts at 7am, evening heating starts at 6pm. If morning/evening hours is greater than 5/6
                //(hence reaching 12 pm/am) then the extra hours are added on before 7am/6pm.
                int hoursOnFrom12amTo7am = Math.Max(0, hoursOfHeatingMorning.Value - 5);
                int hoursOnFrom7amTo12pm = Math.Min(5, hoursOfHeatingMorning.Value);
                int hoursOnFrom12pmTo6pm = Math.Max(0, hoursOfHeatingEvening.Value - 6);
                int hoursOnFrom6pmTo12am = Math.Min(6, hoursOfHeatingEvening.Value);
                //Hours between heating turning off in the morning and turning on in the evening
                int firstOffPeriod = (18 - hoursOnFrom12pmTo6pm) - (7 + hoursOnFrom7amTo12pm);
                //Hours between midnight and heating turning on in the morning, plus hours between heating turning off
                //in the evening and midnight 
                int secondOffPeriod = (7 - hoursOnFrom12amTo7am) + (6 - hoursOnFrom6pmTo12am);
                return new[] { firstOffPeriod, secondOffPeriod };
            }

            return null;
        }

        private static BreFloorType? GetBreFloorType(FloorConstruction? floorConstruction, FloorInsulated? floorInsulated)
        {
            return floorConstruction switch
            {
                FloorConstruction.SuspendedTimber => floorInsulated switch
                {
                    FloorInsulated.Yes => BreFloorType.SuspendedFloorWithInsulation,
                    FloorInsulated.No => BreFloorType.SuspendedFloorWithoutInsulation,
                    //peer-reviewed assumption:
                    FloorInsulated.DoNotKnow => BreFloorType.SuspendedFloorWithoutInsulation,
                    _ => throw new ArgumentOutOfRangeException()
                },
                FloorConstruction.SolidConcrete => floorInsulated switch
                {
                    FloorInsulated.Yes => BreFloorType.SolidFloorWithInsulation,
                    FloorInsulated.No => BreFloorType.SolidFloorWithoutInsulation,
                    //peer-reviewed assumption:
                    FloorInsulated.DoNotKnow => BreFloorType.SolidFloorWithoutInsulation,
                    _ => throw new ArgumentOutOfRangeException()
                },
                FloorConstruction.Mix => floorInsulated switch
                {
                    //peer-reviewed assumptions:
                    FloorInsulated.Yes => BreFloorType.SuspendedFloorWithInsulation,
                    FloorInsulated.No => BreFloorType.SuspendedFloorWithoutInsulation,
                    FloorInsulated.DoNotKnow => BreFloorType.SuspendedFloorWithoutInsulation,
                    _ => throw new ArgumentOutOfRangeException()
                },
                FloorConstruction.Other => BreFloorType.DontKnow,
                FloorConstruction.DoNotKnow => BreFloorType.DontKnow,
                _ => null
            };
        }

        private static bool GetBreOutsideSpace(HasOutdoorSpace? hasOutdoorSpace)
        {
            return hasOutdoorSpace switch
            {
                HasOutdoorSpace.Yes => true,
                HasOutdoorSpace.No => false,
                HasOutdoorSpace.DoNotKnow => true,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}