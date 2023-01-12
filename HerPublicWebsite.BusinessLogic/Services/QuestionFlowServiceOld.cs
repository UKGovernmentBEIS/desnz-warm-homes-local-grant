using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.BusinessLogic.Services
{
    public interface IQuestionFlowServiceOld
    { 
        public QuestionFlowStepOld PreviousStep(QuestionFlowStepOld page, PropertyData propertyData, QuestionFlowStepOld? entryPoint = null);
        
        public QuestionFlowStepOld NextStep(QuestionFlowStepOld page, PropertyData propertyData, QuestionFlowStepOld? entryPoint = null);
        
        public QuestionFlowStepOld SkipDestination(QuestionFlowStepOld page, PropertyData propertyData, QuestionFlowStepOld? entryPoint = null);
    }

    public class QuestionFlowServiceOld: IQuestionFlowServiceOld
    {
        public QuestionFlowStepOld PreviousStep(
            QuestionFlowStepOld page, 
            PropertyData propertyData, 
            QuestionFlowStepOld? entryPoint = null)
        {
            return page switch
            {
                QuestionFlowStepOld.NewOrReturningUser => NewOrReturningUserBackDestination(),
                QuestionFlowStepOld.OwnershipStatus => OwnershipStatusBackDestination(),
                QuestionFlowStepOld.Country => CountryBackDestination(),
                QuestionFlowStepOld.FindEpc => FindEpcBackDestination(),
                QuestionFlowStepOld.ServiceUnsuitable => ServiceUnsuitableBackDestination(propertyData),
                QuestionFlowStepOld.AskForPostcode => AskForPostcodeBackDestination(),
                QuestionFlowStepOld.ConfirmAddress => ConfirmAddressBackDestination(),
                QuestionFlowStepOld.ConfirmEpcDetails => ConfirmEpcDetailsBackDestination(),
                QuestionFlowStepOld.NoEpcFound => NoEpcFoundBackDestination(),
                QuestionFlowStepOld.PropertyType => PropertyTypeBackDestination(propertyData, entryPoint),
                QuestionFlowStepOld.HouseType => HouseTypeBackDestination(),
                QuestionFlowStepOld.BungalowType => BungalowTypeBackDestination(),
                QuestionFlowStepOld.FlatType => FlatTypeBackDestination(),
                QuestionFlowStepOld.HomeAge => HomeAgeBackDestination(propertyData, entryPoint),
                QuestionFlowStepOld.CheckYourUnchangeableAnswers => CheckYourUnchangeableAnswersBackDestination(),
                QuestionFlowStepOld.WallConstruction => WallConstructionBackDestination(propertyData, entryPoint),
                QuestionFlowStepOld.CavityWallsInsulated => CavityWallsInsulatedBackDestination(entryPoint),
                QuestionFlowStepOld.SolidWallsInsulated => SolidWallsInsulatedBackDestination(propertyData, entryPoint),
                QuestionFlowStepOld.FloorConstruction => FloorConstructionBackDestination(propertyData, entryPoint),
                QuestionFlowStepOld.FloorInsulated => FloorInsulatedBackDestination(entryPoint),
                QuestionFlowStepOld.RoofConstruction => RoofConstructionBackDestination(propertyData, entryPoint),
                QuestionFlowStepOld.LoftSpace => LoftSpaceBackDestination(entryPoint),
                QuestionFlowStepOld.LoftAccess => LoftAccessBackDestination(entryPoint),
                QuestionFlowStepOld.RoofInsulated => RoofInsulatedBackDestination(entryPoint),
                QuestionFlowStepOld.OutdoorSpace => OutdoorSpaceBackDestination(entryPoint),
                QuestionFlowStepOld.GlazingType => GlazingTypeBackDestination(propertyData, entryPoint),
                QuestionFlowStepOld.HeatingType => HeatingTypeBackDestination(entryPoint),
                QuestionFlowStepOld.OtherHeatingType => OtherHeatingTypeBackDestination(entryPoint),
                QuestionFlowStepOld.HotWaterCylinder => HotWaterCylinderBackDestination(entryPoint),
                QuestionFlowStepOld.NumberOfOccupants => NumberOfOccupantsBackDestination(propertyData, entryPoint),
                QuestionFlowStepOld.HeatingPattern => HeatingPatternBackDestination(entryPoint),
                QuestionFlowStepOld.Temperature => TemperatureBackDestination(entryPoint),
                QuestionFlowStepOld.AnswerSummary => AnswerSummaryBackDestination(),
                QuestionFlowStepOld.NoRecommendations => NoRecommendationsBackDestination(),
                QuestionFlowStepOld.YourRecommendations => YourRecommendationsBackDestination(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public QuestionFlowStepOld NextStep(QuestionFlowStepOld page, PropertyData propertyData, QuestionFlowStepOld? entryPoint = null)
        {
            return page switch
            {
                QuestionFlowStepOld.NewOrReturningUser => NewOrReturningUserForwardDestination(),
                QuestionFlowStepOld.OwnershipStatus => OwnershipStatusForwardDestination(propertyData),
                QuestionFlowStepOld.Country => CountryForwardDestination(propertyData),
                QuestionFlowStepOld.FindEpc => FindEpcForwardDestination(propertyData),
                QuestionFlowStepOld.AskForPostcode => AskForPostcodeForwardDestination(propertyData),
                QuestionFlowStepOld.ConfirmAddress => ConfirmAddressForwardDestination(propertyData),
                QuestionFlowStepOld.ConfirmEpcDetails => ConfirmEpcDetailsForwardDestination(propertyData),
                QuestionFlowStepOld.NoEpcFound => NoEpcFoundForwardDestination(),
                QuestionFlowStepOld.PropertyType => PropertyTypeForwardDestination(propertyData),
                QuestionFlowStepOld.HouseType => HouseTypeForwardDestination(entryPoint),
                QuestionFlowStepOld.BungalowType => BungalowTypeForwardDestination(entryPoint),
                QuestionFlowStepOld.FlatType => FlatTypeForwardDestination(entryPoint),
                QuestionFlowStepOld.HomeAge => HomeAgeForwardDestination(),
                QuestionFlowStepOld.CheckYourUnchangeableAnswers => CheckYourUnchangeableAnswersForwardDestination(),
                QuestionFlowStepOld.WallConstruction => WallConstructionForwardDestination(propertyData, entryPoint),
                QuestionFlowStepOld.CavityWallsInsulated => CavityWallsInsulatedForwardDestination(propertyData, entryPoint),
                QuestionFlowStepOld.SolidWallsInsulated => SolidWallsInsulatedForwardDestination(propertyData, entryPoint),
                QuestionFlowStepOld.FloorConstruction => FloorConstructionForwardDestination(propertyData, entryPoint),
                QuestionFlowStepOld.FloorInsulated => FloorInsulatedForwardDestination(propertyData, entryPoint),
                QuestionFlowStepOld.RoofConstruction => RoofConstructionForwardDestination(propertyData, entryPoint),
                QuestionFlowStepOld.LoftSpace => LoftSpaceForwardDestination(propertyData, entryPoint),
                QuestionFlowStepOld.LoftAccess => LoftAccessForwardDestination(propertyData, entryPoint),
                QuestionFlowStepOld.RoofInsulated => RoofInsulatedForwardDestination(entryPoint),
                QuestionFlowStepOld.OutdoorSpace => OutdoorSpaceForwardDestination(entryPoint),
                QuestionFlowStepOld.GlazingType => GlazingTypeForwardDestination(entryPoint),
                QuestionFlowStepOld.HeatingType => HeatingTypeForwardDestination(propertyData, entryPoint),
                QuestionFlowStepOld.OtherHeatingType => OtherHeatingTypeForwardDestination(entryPoint),
                QuestionFlowStepOld.HotWaterCylinder => HotWaterCylinderForwardDestination(entryPoint),
                QuestionFlowStepOld.NumberOfOccupants => NumberOfOccupantsForwardDestination(entryPoint),
                QuestionFlowStepOld.HeatingPattern => HeatingPatternForwardDestination(entryPoint),
                QuestionFlowStepOld.Temperature => TemperatureForwardDestination(),
                QuestionFlowStepOld.AnswerSummary => AnswerSummaryForwardDestination(propertyData),
                _ => throw new ArgumentOutOfRangeException(nameof(page), page, null)
            };
        }

        public QuestionFlowStepOld SkipDestination(QuestionFlowStepOld page, PropertyData propertyData, QuestionFlowStepOld? entryPoint = null)
        {
            return page switch
            {
                QuestionFlowStepOld.AskForPostcode => AskForPostcodeSkipDestination(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private QuestionFlowStepOld NewOrReturningUserBackDestination()
        {
            return QuestionFlowStepOld.Start;
        }

        private QuestionFlowStepOld CountryBackDestination()
        {
            return QuestionFlowStepOld.NewOrReturningUser;
        }

        private QuestionFlowStepOld OwnershipStatusBackDestination()
        {
            return QuestionFlowStepOld.Country;
        }

        private QuestionFlowStepOld ServiceUnsuitableBackDestination(PropertyData propertyData)
        {
            return propertyData switch
            {
                { Country: not Country.England and not Country.Wales }
                    => QuestionFlowStepOld.Country,
                { OwnershipStatus: OwnershipStatus.PrivateTenancy }
                    => QuestionFlowStepOld.OwnershipStatus,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private QuestionFlowStepOld FindEpcBackDestination()
        {
            return QuestionFlowStepOld.OwnershipStatus;
        }
        
        private QuestionFlowStepOld AskForPostcodeBackDestination()
        {
            return QuestionFlowStepOld.FindEpc;
        }

        private QuestionFlowStepOld ConfirmAddressBackDestination()
        {
            return QuestionFlowStepOld.AskForPostcode;
        }

        private QuestionFlowStepOld ConfirmEpcDetailsBackDestination()
        {
            return QuestionFlowStepOld.AskForPostcode;
        }

        private QuestionFlowStepOld NoEpcFoundBackDestination()
        {
            return QuestionFlowStepOld.AskForPostcode;
        }

        private QuestionFlowStepOld PropertyTypeBackDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            if (entryPoint is QuestionFlowStepOld.PropertyType)
            {
                return QuestionFlowStepOld.CheckYourUnchangeableAnswers;
            }
            
            if (propertyData.Epc != null)
            {
                if (propertyData.Epc.ContainsPropertyTypeAndAge())
                {
                    return QuestionFlowStepOld.ConfirmEpcDetails;
                }
                return QuestionFlowStepOld.AskForPostcode;
            }

            if (propertyData.SearchForEpc == SearchForEpc.Yes)
            {
                return QuestionFlowStepOld.NoEpcFound;    
            }

            return QuestionFlowStepOld.FindEpc;
        }

        private QuestionFlowStepOld HouseTypeBackDestination()
        {
            return QuestionFlowStepOld.PropertyType;
        }

        private QuestionFlowStepOld BungalowTypeBackDestination()
        {
            return QuestionFlowStepOld.PropertyType;
        }

        private QuestionFlowStepOld FlatTypeBackDestination()
        {
            return QuestionFlowStepOld.PropertyType;
        }

        private QuestionFlowStepOld HomeAgeBackDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.HomeAge
                ? QuestionFlowStepOld.CheckYourUnchangeableAnswers
                : propertyData.PropertyType switch
                {
                    PropertyType.House => 
                        QuestionFlowStepOld.HouseType,
                    PropertyType.Bungalow => 
                        QuestionFlowStepOld.BungalowType,
                    PropertyType.ApartmentFlatOrMaisonette => 
                        QuestionFlowStepOld.FlatType,
                    _ => throw new ArgumentOutOfRangeException()
                };
        }
        
        private QuestionFlowStepOld CheckYourUnchangeableAnswersBackDestination()
        {
            return QuestionFlowStepOld.HomeAge;
        }

        private QuestionFlowStepOld WallConstructionBackDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            if (entryPoint is QuestionFlowStepOld.WallConstruction)
            {
                return QuestionFlowStepOld.AnswerSummary;
            }

            if (propertyData.EpcDetailsConfirmed == EpcDetailsConfirmed.Yes)
            {
                return QuestionFlowStepOld.ConfirmEpcDetails;
            }
            return QuestionFlowStepOld.CheckYourUnchangeableAnswers;
        }

        private QuestionFlowStepOld CavityWallsInsulatedBackDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.CavityWallsInsulated
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.WallConstruction;
        }

        private QuestionFlowStepOld SolidWallsInsulatedBackDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.SolidWallsInsulated
                ? QuestionFlowStepOld.AnswerSummary
                : propertyData.WallConstruction switch
                {
                    WallConstruction.Mixed => 
                        QuestionFlowStepOld.CavityWallsInsulated,
                    WallConstruction.Solid => 
                        QuestionFlowStepOld.WallConstruction,
                    _ => throw new ArgumentOutOfRangeException()
                };
        }

        private QuestionFlowStepOld FloorConstructionBackDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.FloorConstruction
                ? QuestionFlowStepOld.AnswerSummary
                : propertyData.WallConstruction switch
                {
                    WallConstruction.Solid or WallConstruction.Mixed =>
                        QuestionFlowStepOld.SolidWallsInsulated,
                    WallConstruction.Cavity =>
                        QuestionFlowStepOld.CavityWallsInsulated,
                    _ => QuestionFlowStepOld.WallConstruction
                };
        }

        private QuestionFlowStepOld FloorInsulatedBackDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.FloorInsulated
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.FloorConstruction;
        }

        private QuestionFlowStepOld RoofConstructionBackDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            if (entryPoint is QuestionFlowStepOld.RoofConstruction)
            {
                return QuestionFlowStepOld.AnswerSummary;
            }

            if (propertyData.HasFloor())
            {
                return propertyData.FloorConstruction switch
                {
                    FloorConstruction.SuspendedTimber or FloorConstruction.SolidConcrete or FloorConstruction.Mix =>
                        QuestionFlowStepOld.FloorInsulated,
                    _ => QuestionFlowStepOld.FloorConstruction
                };
            }
            
            return propertyData.WallConstruction switch
            {
                WallConstruction.Solid or WallConstruction.Mixed =>
                    QuestionFlowStepOld.SolidWallsInsulated,
                WallConstruction.Cavity =>
                    QuestionFlowStepOld.CavityWallsInsulated,
                _ => QuestionFlowStepOld.WallConstruction
            };
        }

        private QuestionFlowStepOld LoftSpaceBackDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.LoftSpace
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.RoofConstruction;
        }
        
        private QuestionFlowStepOld LoftAccessBackDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.LoftAccess
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.LoftSpace;
        }

        private QuestionFlowStepOld RoofInsulatedBackDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.RoofInsulated
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.LoftAccess;
        }

        private QuestionFlowStepOld GlazingTypeBackDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            if (entryPoint is QuestionFlowStepOld.GlazingType)
            {
                return QuestionFlowStepOld.AnswerSummary;
            }

            if (propertyData.HasRoof())
            {
                return propertyData switch
                {
                    { RoofConstruction: RoofConstruction.Flat }
                        => QuestionFlowStepOld.RoofConstruction,
                    { LoftSpace: not LoftSpace.Yes }
                        => QuestionFlowStepOld.LoftSpace,
                    { LoftAccess: not LoftAccess.Yes }
                        => QuestionFlowStepOld.LoftAccess,
                    _ => QuestionFlowStepOld.RoofInsulated
                };
            }

            if (propertyData.HasFloor())
            {
                return propertyData.FloorConstruction switch
                {
                    FloorConstruction.SuspendedTimber or FloorConstruction.SolidConcrete or FloorConstruction.Mix =>
                        QuestionFlowStepOld.FloorInsulated,
                    _ => QuestionFlowStepOld.FloorConstruction
                };
            }
            
            return propertyData.WallConstruction switch
            {
                WallConstruction.Solid or WallConstruction.Mixed =>
                    QuestionFlowStepOld.SolidWallsInsulated,
                WallConstruction.Cavity =>
                    QuestionFlowStepOld.CavityWallsInsulated,
                _ => QuestionFlowStepOld.WallConstruction
            };
        }

        private QuestionFlowStepOld OutdoorSpaceBackDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.OutdoorSpace
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.GlazingType;
        }

        private QuestionFlowStepOld HeatingTypeBackDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.HeatingType
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.OutdoorSpace;
        }

        private QuestionFlowStepOld OtherHeatingTypeBackDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.OtherHeatingType
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.HeatingType;
        }

        private QuestionFlowStepOld HotWaterCylinderBackDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.HotWaterCylinder
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.HeatingType;
        }

        private QuestionFlowStepOld NumberOfOccupantsBackDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.NumberOfOccupants
                ? QuestionFlowStepOld.AnswerSummary
                : propertyData.HeatingType switch
                {
                    HeatingType.Storage or HeatingType.DirectActionElectric or HeatingType.HeatPump
                        or HeatingType.DoNotKnow
                        => QuestionFlowStepOld.HeatingType,
                    HeatingType.GasBoiler or HeatingType.OilBoiler or HeatingType.LpgBoiler
                        => QuestionFlowStepOld.HotWaterCylinder,
                    HeatingType.Other
                        => QuestionFlowStepOld.OtherHeatingType,
                    _ => throw new ArgumentOutOfRangeException()
                };
        }

        private QuestionFlowStepOld HeatingPatternBackDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.HeatingPattern
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.NumberOfOccupants;
        }

        private QuestionFlowStepOld TemperatureBackDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is QuestionFlowStepOld.Temperature
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.HeatingPattern;
        }

        private QuestionFlowStepOld AnswerSummaryBackDestination()
        {
            return QuestionFlowStepOld.Temperature;
        }

        private QuestionFlowStepOld NoRecommendationsBackDestination()
        {
            return QuestionFlowStepOld.AnswerSummary;
        }

        private QuestionFlowStepOld YourRecommendationsBackDestination()
        {
            return QuestionFlowStepOld.AnswerSummary;
        }

        private QuestionFlowStepOld NewOrReturningUserForwardDestination()
        {
            return QuestionFlowStepOld.Country;
        }
        
        private QuestionFlowStepOld CountryForwardDestination(PropertyData propertyData)
        {
            return propertyData.Country is not Country.England and not Country.Wales 
                ? QuestionFlowStepOld.ServiceUnsuitable
                : QuestionFlowStepOld.OwnershipStatus;
        }

        private QuestionFlowStepOld OwnershipStatusForwardDestination(PropertyData propertyData)
        {
            return propertyData.OwnershipStatus is OwnershipStatus.PrivateTenancy 
                ? QuestionFlowStepOld.ServiceUnsuitable
                : QuestionFlowStepOld.FindEpc;
        }

        private QuestionFlowStepOld FindEpcForwardDestination(PropertyData propertyData)
        {
            if (propertyData.SearchForEpc == SearchForEpc.Yes)
            {
                return QuestionFlowStepOld.AskForPostcode;
            }
            
            return QuestionFlowStepOld.PropertyType;
        }

        private QuestionFlowStepOld AskForPostcodeForwardDestination(PropertyData propertyData)
        {
            return QuestionFlowStepOld.ConfirmAddress;
        }

        private QuestionFlowStepOld ConfirmAddressForwardDestination(PropertyData propertyData)
        {
            var epc = propertyData.Epc;
            if (epc != null)
            {
                if (epc.ContainsPropertyTypeAndAge())
                {
                    return QuestionFlowStepOld.ConfirmEpcDetails;
                }
                return QuestionFlowStepOld.PropertyType;
            }
            return QuestionFlowStepOld.NoEpcFound;
        }

        private QuestionFlowStepOld ConfirmEpcDetailsForwardDestination(PropertyData propertyData)
        {
            if (propertyData.EpcDetailsConfirmed == EpcDetailsConfirmed.Yes)
            {
                return QuestionFlowStepOld.WallConstruction;
            }
            return QuestionFlowStepOld.PropertyType;
        }

        private QuestionFlowStepOld NoEpcFoundForwardDestination()
        {
            return QuestionFlowStepOld.PropertyType;
        }
        
        private QuestionFlowStepOld PropertyTypeForwardDestination(PropertyData propertyData)
        {
            return propertyData.PropertyType switch
            {
                PropertyType.House =>
                    QuestionFlowStepOld.HouseType,
                PropertyType.Bungalow =>
                    QuestionFlowStepOld.BungalowType,
                PropertyType.ApartmentFlatOrMaisonette =>
                    QuestionFlowStepOld.FlatType,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        private QuestionFlowStepOld HouseTypeForwardDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStepOld.CheckYourUnchangeableAnswers
                : QuestionFlowStepOld.HomeAge;
        }

        private QuestionFlowStepOld BungalowTypeForwardDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStepOld.CheckYourUnchangeableAnswers
                : QuestionFlowStepOld.HomeAge;
        }

        private QuestionFlowStepOld FlatTypeForwardDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStepOld.CheckYourUnchangeableAnswers
                : QuestionFlowStepOld.HomeAge;
        }

        private QuestionFlowStepOld HomeAgeForwardDestination()
        {
            return QuestionFlowStepOld.CheckYourUnchangeableAnswers;
        }

        private QuestionFlowStepOld CheckYourUnchangeableAnswersForwardDestination()
        {
            return QuestionFlowStepOld.WallConstruction;
        }
        
        private QuestionFlowStepOld WallConstructionForwardDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {

            if (propertyData.WallConstruction is WallConstruction.Cavity or WallConstruction.Mixed)
            {
                return QuestionFlowStepOld.CavityWallsInsulated;
            }

            if (propertyData.WallConstruction == WallConstruction.Solid)
            {
                return QuestionFlowStepOld.SolidWallsInsulated;
            }
            
            if (entryPoint is not null)
            {
                return QuestionFlowStepOld.AnswerSummary;
            }

            // These options below are for people who have chosen "Don't know" to "What type of walls do you have?"
            if (propertyData.HasFloor())
            {
                return QuestionFlowStepOld.FloorConstruction;
            }

            if (propertyData.HasRoof())
            {
                return QuestionFlowStepOld.RoofConstruction;
            }

            return QuestionFlowStepOld.GlazingType;
        }

        private QuestionFlowStepOld CavityWallsInsulatedForwardDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            
            if (entryPoint is QuestionFlowStepOld.CavityWallsInsulated)
            {
                return QuestionFlowStepOld.AnswerSummary;
            }

            if (propertyData.WallConstruction is WallConstruction.Mixed)
            {
                return QuestionFlowStepOld.SolidWallsInsulated;
            }
            
            if (entryPoint is not null)
            {
                return QuestionFlowStepOld.AnswerSummary;
            }

            // These options below are for people who have finished the "wall insulation" questions (e.g. who only have cavity walls)
            if (propertyData.HasFloor())
            {
                return QuestionFlowStepOld.FloorConstruction;
            }

            if (propertyData.HasRoof())
            {
                return QuestionFlowStepOld.RoofConstruction;
            }

            return QuestionFlowStepOld.GlazingType;
        }

        private QuestionFlowStepOld SolidWallsInsulatedForwardDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            if (entryPoint is not null)
            {
                return QuestionFlowStepOld.AnswerSummary;
            }

            if (propertyData.HasFloor())
            {
                return QuestionFlowStepOld.FloorConstruction;
            }

            if (propertyData.HasRoof())
            {
                return QuestionFlowStepOld.RoofConstruction;
            }

            return QuestionFlowStepOld.GlazingType;
        }

        private QuestionFlowStepOld FloorConstructionForwardDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {

            if (propertyData.FloorConstruction is FloorConstruction.SolidConcrete or FloorConstruction.SuspendedTimber or FloorConstruction.Mix ) 
            {
                return QuestionFlowStepOld.FloorInsulated;
            }
            
            if (entryPoint is not null)
            {
                return QuestionFlowStepOld.AnswerSummary;
            }

            if (propertyData.HasRoof())
            {
                return QuestionFlowStepOld.RoofConstruction;
            }

            return QuestionFlowStepOld.GlazingType;
        }

        private QuestionFlowStepOld FloorInsulatedForwardDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            if (entryPoint is not null)
            {
                return QuestionFlowStepOld.AnswerSummary;
            }

            if (propertyData.HasRoof())
            {
                return QuestionFlowStepOld.RoofConstruction;
            }

            return QuestionFlowStepOld.GlazingType;
        }

        private QuestionFlowStepOld RoofConstructionForwardDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            if (propertyData.RoofConstruction is RoofConstruction.Mixed or RoofConstruction.Pitched)
            {
                return QuestionFlowStepOld.LoftSpace;
            }

            if (entryPoint is not null)
            {
                return QuestionFlowStepOld.AnswerSummary;
            }

            return QuestionFlowStepOld.GlazingType;
        }

        private QuestionFlowStepOld LoftSpaceForwardDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            if (propertyData.LoftSpace is LoftSpace.Yes)
            {
                return QuestionFlowStepOld.LoftAccess;
            }

            if (entryPoint is not null)
            {
                return QuestionFlowStepOld.AnswerSummary;
            }

            return QuestionFlowStepOld.GlazingType;
        }
        
        private QuestionFlowStepOld LoftAccessForwardDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            if (propertyData.LoftAccess is LoftAccess.Yes)
            {
                return QuestionFlowStepOld.RoofInsulated;
            }

            if (entryPoint is not null)
            {
                return QuestionFlowStepOld.AnswerSummary;
            }

            return QuestionFlowStepOld.GlazingType;
        }

        private QuestionFlowStepOld RoofInsulatedForwardDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.GlazingType;
        }

        private QuestionFlowStepOld GlazingTypeForwardDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.OutdoorSpace;
        }

        private QuestionFlowStepOld OutdoorSpaceForwardDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.HeatingType;
        }

        private QuestionFlowStepOld HeatingTypeForwardDestination(PropertyData propertyData, QuestionFlowStepOld? entryPoint)
        {
            if (propertyData.HeatingType == HeatingType.Other)
            {
                return QuestionFlowStepOld.OtherHeatingType;
            }

            if (propertyData.HeatingType is HeatingType.GasBoiler or HeatingType.OilBoiler or HeatingType.LpgBoiler)
            {
                return QuestionFlowStepOld.HotWaterCylinder;
            }

            if (entryPoint is not null)
            {
                return QuestionFlowStepOld.AnswerSummary;
            }

            return QuestionFlowStepOld.NumberOfOccupants;
        }

        private QuestionFlowStepOld OtherHeatingTypeForwardDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.NumberOfOccupants;
        }

        private QuestionFlowStepOld HotWaterCylinderForwardDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.NumberOfOccupants;
        }

        private QuestionFlowStepOld NumberOfOccupantsForwardDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.HeatingPattern;
        }

        private QuestionFlowStepOld HeatingPatternForwardDestination(QuestionFlowStepOld? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStepOld.AnswerSummary
                : QuestionFlowStepOld.Temperature;
        }

        private QuestionFlowStepOld TemperatureForwardDestination()
        {
            return QuestionFlowStepOld.AnswerSummary;
        }

        private QuestionFlowStepOld AnswerSummaryForwardDestination(PropertyData propertyData)
        {
            return propertyData.PropertyRecommendations.Any()
                ? QuestionFlowStepOld.YourRecommendations
                : QuestionFlowStepOld.NoRecommendations;
        }
        
        private QuestionFlowStepOld AskForPostcodeSkipDestination()
        {
            return QuestionFlowStepOld.PropertyType;
        }
    }
}