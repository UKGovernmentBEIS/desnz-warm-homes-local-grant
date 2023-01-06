using SeaPublicWebsite.BusinessLogic.Models;
using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace SeaPublicWebsite.BusinessLogic.Services
{
    public interface IQuestionFlowService
    { 
        public QuestionFlowStep PreviousStep(QuestionFlowStep page, PropertyData propertyData, QuestionFlowStep? entryPoint = null);
        
        public QuestionFlowStep NextStep(QuestionFlowStep page, PropertyData propertyData, QuestionFlowStep? entryPoint = null);
        
        public QuestionFlowStep SkipDestination(QuestionFlowStep page, PropertyData propertyData, QuestionFlowStep? entryPoint = null);
    }

    public class QuestionFlowService: IQuestionFlowService
    {
        public QuestionFlowStep PreviousStep(
            QuestionFlowStep page, 
            PropertyData propertyData, 
            QuestionFlowStep? entryPoint = null)
        {
            return page switch
            {
                QuestionFlowStep.NewOrReturningUser => NewOrReturningUserBackDestination(),
                QuestionFlowStep.OwnershipStatus => OwnershipStatusBackDestination(),
                QuestionFlowStep.Country => CountryBackDestination(),
                QuestionFlowStep.FindEpc => FindEpcBackDestination(),
                QuestionFlowStep.ServiceUnsuitable => ServiceUnsuitableBackDestination(propertyData),
                QuestionFlowStep.AskForPostcode => AskForPostcodeBackDestination(),
                QuestionFlowStep.ConfirmAddress => ConfirmAddressBackDestination(),
                QuestionFlowStep.ConfirmEpcDetails => ConfirmEpcDetailsBackDestination(),
                QuestionFlowStep.NoEpcFound => NoEpcFoundBackDestination(),
                QuestionFlowStep.PropertyType => PropertyTypeBackDestination(propertyData, entryPoint),
                QuestionFlowStep.HouseType => HouseTypeBackDestination(),
                QuestionFlowStep.BungalowType => BungalowTypeBackDestination(),
                QuestionFlowStep.FlatType => FlatTypeBackDestination(),
                QuestionFlowStep.HomeAge => HomeAgeBackDestination(propertyData, entryPoint),
                QuestionFlowStep.CheckYourUnchangeableAnswers => CheckYourUnchangeableAnswersBackDestination(),
                QuestionFlowStep.WallConstruction => WallConstructionBackDestination(propertyData, entryPoint),
                QuestionFlowStep.CavityWallsInsulated => CavityWallsInsulatedBackDestination(entryPoint),
                QuestionFlowStep.SolidWallsInsulated => SolidWallsInsulatedBackDestination(propertyData, entryPoint),
                QuestionFlowStep.FloorConstruction => FloorConstructionBackDestination(propertyData, entryPoint),
                QuestionFlowStep.FloorInsulated => FloorInsulatedBackDestination(entryPoint),
                QuestionFlowStep.RoofConstruction => RoofConstructionBackDestination(propertyData, entryPoint),
                QuestionFlowStep.LoftSpace => LoftSpaceBackDestination(entryPoint),
                QuestionFlowStep.LoftAccess => LoftAccessBackDestination(entryPoint),
                QuestionFlowStep.RoofInsulated => RoofInsulatedBackDestination(entryPoint),
                QuestionFlowStep.OutdoorSpace => OutdoorSpaceBackDestination(entryPoint),
                QuestionFlowStep.GlazingType => GlazingTypeBackDestination(propertyData, entryPoint),
                QuestionFlowStep.HeatingType => HeatingTypeBackDestination(entryPoint),
                QuestionFlowStep.OtherHeatingType => OtherHeatingTypeBackDestination(entryPoint),
                QuestionFlowStep.HotWaterCylinder => HotWaterCylinderBackDestination(entryPoint),
                QuestionFlowStep.NumberOfOccupants => NumberOfOccupantsBackDestination(propertyData, entryPoint),
                QuestionFlowStep.HeatingPattern => HeatingPatternBackDestination(entryPoint),
                QuestionFlowStep.Temperature => TemperatureBackDestination(entryPoint),
                QuestionFlowStep.AnswerSummary => AnswerSummaryBackDestination(),
                QuestionFlowStep.NoRecommendations => NoRecommendationsBackDestination(),
                QuestionFlowStep.YourRecommendations => YourRecommendationsBackDestination(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public QuestionFlowStep NextStep(QuestionFlowStep page, PropertyData propertyData, QuestionFlowStep? entryPoint = null)
        {
            return page switch
            {
                QuestionFlowStep.NewOrReturningUser => NewOrReturningUserForwardDestination(),
                QuestionFlowStep.OwnershipStatus => OwnershipStatusForwardDestination(propertyData),
                QuestionFlowStep.Country => CountryForwardDestination(propertyData),
                QuestionFlowStep.FindEpc => FindEpcForwardDestination(propertyData),
                QuestionFlowStep.AskForPostcode => AskForPostcodeForwardDestination(propertyData),
                QuestionFlowStep.ConfirmAddress => ConfirmAddressForwardDestination(propertyData),
                QuestionFlowStep.ConfirmEpcDetails => ConfirmEpcDetailsForwardDestination(propertyData),
                QuestionFlowStep.NoEpcFound => NoEpcFoundForwardDestination(),
                QuestionFlowStep.PropertyType => PropertyTypeForwardDestination(propertyData),
                QuestionFlowStep.HouseType => HouseTypeForwardDestination(entryPoint),
                QuestionFlowStep.BungalowType => BungalowTypeForwardDestination(entryPoint),
                QuestionFlowStep.FlatType => FlatTypeForwardDestination(entryPoint),
                QuestionFlowStep.HomeAge => HomeAgeForwardDestination(),
                QuestionFlowStep.CheckYourUnchangeableAnswers => CheckYourUnchangeableAnswersForwardDestination(),
                QuestionFlowStep.WallConstruction => WallConstructionForwardDestination(propertyData, entryPoint),
                QuestionFlowStep.CavityWallsInsulated => CavityWallsInsulatedForwardDestination(propertyData, entryPoint),
                QuestionFlowStep.SolidWallsInsulated => SolidWallsInsulatedForwardDestination(propertyData, entryPoint),
                QuestionFlowStep.FloorConstruction => FloorConstructionForwardDestination(propertyData, entryPoint),
                QuestionFlowStep.FloorInsulated => FloorInsulatedForwardDestination(propertyData, entryPoint),
                QuestionFlowStep.RoofConstruction => RoofConstructionForwardDestination(propertyData, entryPoint),
                QuestionFlowStep.LoftSpace => LoftSpaceForwardDestination(propertyData, entryPoint),
                QuestionFlowStep.LoftAccess => LoftAccessForwardDestination(propertyData, entryPoint),
                QuestionFlowStep.RoofInsulated => RoofInsulatedForwardDestination(entryPoint),
                QuestionFlowStep.OutdoorSpace => OutdoorSpaceForwardDestination(entryPoint),
                QuestionFlowStep.GlazingType => GlazingTypeForwardDestination(entryPoint),
                QuestionFlowStep.HeatingType => HeatingTypeForwardDestination(propertyData, entryPoint),
                QuestionFlowStep.OtherHeatingType => OtherHeatingTypeForwardDestination(entryPoint),
                QuestionFlowStep.HotWaterCylinder => HotWaterCylinderForwardDestination(entryPoint),
                QuestionFlowStep.NumberOfOccupants => NumberOfOccupantsForwardDestination(entryPoint),
                QuestionFlowStep.HeatingPattern => HeatingPatternForwardDestination(entryPoint),
                QuestionFlowStep.Temperature => TemperatureForwardDestination(),
                QuestionFlowStep.AnswerSummary => AnswerSummaryForwardDestination(propertyData),
                _ => throw new ArgumentOutOfRangeException(nameof(page), page, null)
            };
        }

        public QuestionFlowStep SkipDestination(QuestionFlowStep page, PropertyData propertyData, QuestionFlowStep? entryPoint = null)
        {
            return page switch
            {
                QuestionFlowStep.AskForPostcode => AskForPostcodeSkipDestination(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private QuestionFlowStep NewOrReturningUserBackDestination()
        {
            return QuestionFlowStep.Start;
        }

        private QuestionFlowStep CountryBackDestination()
        {
            return QuestionFlowStep.NewOrReturningUser;
        }

        private QuestionFlowStep OwnershipStatusBackDestination()
        {
            return QuestionFlowStep.Country;
        }

        private QuestionFlowStep ServiceUnsuitableBackDestination(PropertyData propertyData)
        {
            return propertyData switch
            {
                { Country: not Country.England and not Country.Wales }
                    => QuestionFlowStep.Country,
                { OwnershipStatus: OwnershipStatus.PrivateTenancy }
                    => QuestionFlowStep.OwnershipStatus,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private QuestionFlowStep FindEpcBackDestination()
        {
            return QuestionFlowStep.OwnershipStatus;
        }
        
        private QuestionFlowStep AskForPostcodeBackDestination()
        {
            return QuestionFlowStep.FindEpc;
        }

        private QuestionFlowStep ConfirmAddressBackDestination()
        {
            return QuestionFlowStep.AskForPostcode;
        }

        private QuestionFlowStep ConfirmEpcDetailsBackDestination()
        {
            return QuestionFlowStep.AskForPostcode;
        }

        private QuestionFlowStep NoEpcFoundBackDestination()
        {
            return QuestionFlowStep.AskForPostcode;
        }

        private QuestionFlowStep PropertyTypeBackDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            if (entryPoint is QuestionFlowStep.PropertyType)
            {
                return QuestionFlowStep.CheckYourUnchangeableAnswers;
            }
            
            if (propertyData.Epc != null)
            {
                if (propertyData.Epc.ContainsPropertyTypeAndAge())
                {
                    return QuestionFlowStep.ConfirmEpcDetails;
                }
                return QuestionFlowStep.AskForPostcode;
            }

            if (propertyData.SearchForEpc == SearchForEpc.Yes)
            {
                return QuestionFlowStep.NoEpcFound;    
            }

            return QuestionFlowStep.FindEpc;
        }

        private QuestionFlowStep HouseTypeBackDestination()
        {
            return QuestionFlowStep.PropertyType;
        }

        private QuestionFlowStep BungalowTypeBackDestination()
        {
            return QuestionFlowStep.PropertyType;
        }

        private QuestionFlowStep FlatTypeBackDestination()
        {
            return QuestionFlowStep.PropertyType;
        }

        private QuestionFlowStep HomeAgeBackDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.HomeAge
                ? QuestionFlowStep.CheckYourUnchangeableAnswers
                : propertyData.PropertyType switch
                {
                    PropertyType.House => 
                        QuestionFlowStep.HouseType,
                    PropertyType.Bungalow => 
                        QuestionFlowStep.BungalowType,
                    PropertyType.ApartmentFlatOrMaisonette => 
                        QuestionFlowStep.FlatType,
                    _ => throw new ArgumentOutOfRangeException()
                };
        }
        
        private QuestionFlowStep CheckYourUnchangeableAnswersBackDestination()
        {
            return QuestionFlowStep.HomeAge;
        }

        private QuestionFlowStep WallConstructionBackDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            if (entryPoint is QuestionFlowStep.WallConstruction)
            {
                return QuestionFlowStep.AnswerSummary;
            }

            if (propertyData.EpcDetailsConfirmed == EpcDetailsConfirmed.Yes)
            {
                return QuestionFlowStep.ConfirmEpcDetails;
            }
            return QuestionFlowStep.CheckYourUnchangeableAnswers;
        }

        private QuestionFlowStep CavityWallsInsulatedBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.CavityWallsInsulated
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.WallConstruction;
        }

        private QuestionFlowStep SolidWallsInsulatedBackDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.SolidWallsInsulated
                ? QuestionFlowStep.AnswerSummary
                : propertyData.WallConstruction switch
                {
                    WallConstruction.Mixed => 
                        QuestionFlowStep.CavityWallsInsulated,
                    WallConstruction.Solid => 
                        QuestionFlowStep.WallConstruction,
                    _ => throw new ArgumentOutOfRangeException()
                };
        }

        private QuestionFlowStep FloorConstructionBackDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.FloorConstruction
                ? QuestionFlowStep.AnswerSummary
                : propertyData.WallConstruction switch
                {
                    WallConstruction.Solid or WallConstruction.Mixed =>
                        QuestionFlowStep.SolidWallsInsulated,
                    WallConstruction.Cavity =>
                        QuestionFlowStep.CavityWallsInsulated,
                    _ => QuestionFlowStep.WallConstruction
                };
        }

        private QuestionFlowStep FloorInsulatedBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.FloorInsulated
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.FloorConstruction;
        }

        private QuestionFlowStep RoofConstructionBackDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            if (entryPoint is QuestionFlowStep.RoofConstruction)
            {
                return QuestionFlowStep.AnswerSummary;
            }

            if (propertyData.HasFloor())
            {
                return propertyData.FloorConstruction switch
                {
                    FloorConstruction.SuspendedTimber or FloorConstruction.SolidConcrete or FloorConstruction.Mix =>
                        QuestionFlowStep.FloorInsulated,
                    _ => QuestionFlowStep.FloorConstruction
                };
            }
            
            return propertyData.WallConstruction switch
            {
                WallConstruction.Solid or WallConstruction.Mixed =>
                    QuestionFlowStep.SolidWallsInsulated,
                WallConstruction.Cavity =>
                    QuestionFlowStep.CavityWallsInsulated,
                _ => QuestionFlowStep.WallConstruction
            };
        }

        private QuestionFlowStep LoftSpaceBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.LoftSpace
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.RoofConstruction;
        }
        
        private QuestionFlowStep LoftAccessBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.LoftAccess
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.LoftSpace;
        }

        private QuestionFlowStep RoofInsulatedBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.RoofInsulated
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.LoftAccess;
        }

        private QuestionFlowStep GlazingTypeBackDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            if (entryPoint is QuestionFlowStep.GlazingType)
            {
                return QuestionFlowStep.AnswerSummary;
            }

            if (propertyData.HasRoof())
            {
                return propertyData switch
                {
                    { RoofConstruction: RoofConstruction.Flat }
                        => QuestionFlowStep.RoofConstruction,
                    { LoftSpace: not LoftSpace.Yes }
                        => QuestionFlowStep.LoftSpace,
                    { LoftAccess: not LoftAccess.Yes }
                        => QuestionFlowStep.LoftAccess,
                    _ => QuestionFlowStep.RoofInsulated
                };
            }

            if (propertyData.HasFloor())
            {
                return propertyData.FloorConstruction switch
                {
                    FloorConstruction.SuspendedTimber or FloorConstruction.SolidConcrete or FloorConstruction.Mix =>
                        QuestionFlowStep.FloorInsulated,
                    _ => QuestionFlowStep.FloorConstruction
                };
            }
            
            return propertyData.WallConstruction switch
            {
                WallConstruction.Solid or WallConstruction.Mixed =>
                    QuestionFlowStep.SolidWallsInsulated,
                WallConstruction.Cavity =>
                    QuestionFlowStep.CavityWallsInsulated,
                _ => QuestionFlowStep.WallConstruction
            };
        }

        private QuestionFlowStep OutdoorSpaceBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.OutdoorSpace
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.GlazingType;
        }

        private QuestionFlowStep HeatingTypeBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.HeatingType
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.OutdoorSpace;
        }

        private QuestionFlowStep OtherHeatingTypeBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.OtherHeatingType
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.HeatingType;
        }

        private QuestionFlowStep HotWaterCylinderBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.HotWaterCylinder
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.HeatingType;
        }

        private QuestionFlowStep NumberOfOccupantsBackDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.NumberOfOccupants
                ? QuestionFlowStep.AnswerSummary
                : propertyData.HeatingType switch
                {
                    HeatingType.Storage or HeatingType.DirectActionElectric or HeatingType.HeatPump
                        or HeatingType.DoNotKnow
                        => QuestionFlowStep.HeatingType,
                    HeatingType.GasBoiler or HeatingType.OilBoiler or HeatingType.LpgBoiler
                        => QuestionFlowStep.HotWaterCylinder,
                    HeatingType.Other
                        => QuestionFlowStep.OtherHeatingType,
                    _ => throw new ArgumentOutOfRangeException()
                };
        }

        private QuestionFlowStep HeatingPatternBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.HeatingPattern
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.NumberOfOccupants;
        }

        private QuestionFlowStep TemperatureBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is QuestionFlowStep.Temperature
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.HeatingPattern;
        }

        private QuestionFlowStep AnswerSummaryBackDestination()
        {
            return QuestionFlowStep.Temperature;
        }

        private QuestionFlowStep NoRecommendationsBackDestination()
        {
            return QuestionFlowStep.AnswerSummary;
        }

        private QuestionFlowStep YourRecommendationsBackDestination()
        {
            return QuestionFlowStep.AnswerSummary;
        }

        private QuestionFlowStep NewOrReturningUserForwardDestination()
        {
            return QuestionFlowStep.Country;
        }
        
        private QuestionFlowStep CountryForwardDestination(PropertyData propertyData)
        {
            return propertyData.Country is not Country.England and not Country.Wales 
                ? QuestionFlowStep.ServiceUnsuitable
                : QuestionFlowStep.OwnershipStatus;
        }

        private QuestionFlowStep OwnershipStatusForwardDestination(PropertyData propertyData)
        {
            return propertyData.OwnershipStatus is OwnershipStatus.PrivateTenancy 
                ? QuestionFlowStep.ServiceUnsuitable
                : QuestionFlowStep.FindEpc;
        }

        private QuestionFlowStep FindEpcForwardDestination(PropertyData propertyData)
        {
            if (propertyData.SearchForEpc == SearchForEpc.Yes)
            {
                return QuestionFlowStep.AskForPostcode;
            }
            
            return QuestionFlowStep.PropertyType;
        }

        private QuestionFlowStep AskForPostcodeForwardDestination(PropertyData propertyData)
        {
            return QuestionFlowStep.ConfirmAddress;
        }

        private QuestionFlowStep ConfirmAddressForwardDestination(PropertyData propertyData)
        {
            var epc = propertyData.Epc;
            if (epc != null)
            {
                if (epc.ContainsPropertyTypeAndAge())
                {
                    return QuestionFlowStep.ConfirmEpcDetails;
                }
                return QuestionFlowStep.PropertyType;
            }
            return QuestionFlowStep.NoEpcFound;
        }

        private QuestionFlowStep ConfirmEpcDetailsForwardDestination(PropertyData propertyData)
        {
            if (propertyData.EpcDetailsConfirmed == EpcDetailsConfirmed.Yes)
            {
                return QuestionFlowStep.WallConstruction;
            }
            return QuestionFlowStep.PropertyType;
        }

        private QuestionFlowStep NoEpcFoundForwardDestination()
        {
            return QuestionFlowStep.PropertyType;
        }
        
        private QuestionFlowStep PropertyTypeForwardDestination(PropertyData propertyData)
        {
            return propertyData.PropertyType switch
            {
                PropertyType.House =>
                    QuestionFlowStep.HouseType,
                PropertyType.Bungalow =>
                    QuestionFlowStep.BungalowType,
                PropertyType.ApartmentFlatOrMaisonette =>
                    QuestionFlowStep.FlatType,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        private QuestionFlowStep HouseTypeForwardDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStep.CheckYourUnchangeableAnswers
                : QuestionFlowStep.HomeAge;
        }

        private QuestionFlowStep BungalowTypeForwardDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStep.CheckYourUnchangeableAnswers
                : QuestionFlowStep.HomeAge;
        }

        private QuestionFlowStep FlatTypeForwardDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStep.CheckYourUnchangeableAnswers
                : QuestionFlowStep.HomeAge;
        }

        private QuestionFlowStep HomeAgeForwardDestination()
        {
            return QuestionFlowStep.CheckYourUnchangeableAnswers;
        }

        private QuestionFlowStep CheckYourUnchangeableAnswersForwardDestination()
        {
            return QuestionFlowStep.WallConstruction;
        }
        
        private QuestionFlowStep WallConstructionForwardDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {

            if (propertyData.WallConstruction is WallConstruction.Cavity or WallConstruction.Mixed)
            {
                return QuestionFlowStep.CavityWallsInsulated;
            }

            if (propertyData.WallConstruction == WallConstruction.Solid)
            {
                return QuestionFlowStep.SolidWallsInsulated;
            }
            
            if (entryPoint is not null)
            {
                return QuestionFlowStep.AnswerSummary;
            }

            // These options below are for people who have chosen "Don't know" to "What type of walls do you have?"
            if (propertyData.HasFloor())
            {
                return QuestionFlowStep.FloorConstruction;
            }

            if (propertyData.HasRoof())
            {
                return QuestionFlowStep.RoofConstruction;
            }

            return QuestionFlowStep.GlazingType;
        }

        private QuestionFlowStep CavityWallsInsulatedForwardDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            
            if (entryPoint is QuestionFlowStep.CavityWallsInsulated)
            {
                return QuestionFlowStep.AnswerSummary;
            }

            if (propertyData.WallConstruction is WallConstruction.Mixed)
            {
                return QuestionFlowStep.SolidWallsInsulated;
            }
            
            if (entryPoint is not null)
            {
                return QuestionFlowStep.AnswerSummary;
            }

            // These options below are for people who have finished the "wall insulation" questions (e.g. who only have cavity walls)
            if (propertyData.HasFloor())
            {
                return QuestionFlowStep.FloorConstruction;
            }

            if (propertyData.HasRoof())
            {
                return QuestionFlowStep.RoofConstruction;
            }

            return QuestionFlowStep.GlazingType;
        }

        private QuestionFlowStep SolidWallsInsulatedForwardDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            if (entryPoint is not null)
            {
                return QuestionFlowStep.AnswerSummary;
            }

            if (propertyData.HasFloor())
            {
                return QuestionFlowStep.FloorConstruction;
            }

            if (propertyData.HasRoof())
            {
                return QuestionFlowStep.RoofConstruction;
            }

            return QuestionFlowStep.GlazingType;
        }

        private QuestionFlowStep FloorConstructionForwardDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {

            if (propertyData.FloorConstruction is FloorConstruction.SolidConcrete or FloorConstruction.SuspendedTimber or FloorConstruction.Mix ) 
            {
                return QuestionFlowStep.FloorInsulated;
            }
            
            if (entryPoint is not null)
            {
                return QuestionFlowStep.AnswerSummary;
            }

            if (propertyData.HasRoof())
            {
                return QuestionFlowStep.RoofConstruction;
            }

            return QuestionFlowStep.GlazingType;
        }

        private QuestionFlowStep FloorInsulatedForwardDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            if (entryPoint is not null)
            {
                return QuestionFlowStep.AnswerSummary;
            }

            if (propertyData.HasRoof())
            {
                return QuestionFlowStep.RoofConstruction;
            }

            return QuestionFlowStep.GlazingType;
        }

        private QuestionFlowStep RoofConstructionForwardDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            if (propertyData.RoofConstruction is RoofConstruction.Mixed or RoofConstruction.Pitched)
            {
                return QuestionFlowStep.LoftSpace;
            }

            if (entryPoint is not null)
            {
                return QuestionFlowStep.AnswerSummary;
            }

            return QuestionFlowStep.GlazingType;
        }

        private QuestionFlowStep LoftSpaceForwardDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            if (propertyData.LoftSpace is LoftSpace.Yes)
            {
                return QuestionFlowStep.LoftAccess;
            }

            if (entryPoint is not null)
            {
                return QuestionFlowStep.AnswerSummary;
            }

            return QuestionFlowStep.GlazingType;
        }
        
        private QuestionFlowStep LoftAccessForwardDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            if (propertyData.LoftAccess is LoftAccess.Yes)
            {
                return QuestionFlowStep.RoofInsulated;
            }

            if (entryPoint is not null)
            {
                return QuestionFlowStep.AnswerSummary;
            }

            return QuestionFlowStep.GlazingType;
        }

        private QuestionFlowStep RoofInsulatedForwardDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.GlazingType;
        }

        private QuestionFlowStep GlazingTypeForwardDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.OutdoorSpace;
        }

        private QuestionFlowStep OutdoorSpaceForwardDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.HeatingType;
        }

        private QuestionFlowStep HeatingTypeForwardDestination(PropertyData propertyData, QuestionFlowStep? entryPoint)
        {
            if (propertyData.HeatingType == HeatingType.Other)
            {
                return QuestionFlowStep.OtherHeatingType;
            }

            if (propertyData.HeatingType is HeatingType.GasBoiler or HeatingType.OilBoiler or HeatingType.LpgBoiler)
            {
                return QuestionFlowStep.HotWaterCylinder;
            }

            if (entryPoint is not null)
            {
                return QuestionFlowStep.AnswerSummary;
            }

            return QuestionFlowStep.NumberOfOccupants;
        }

        private QuestionFlowStep OtherHeatingTypeForwardDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.NumberOfOccupants;
        }

        private QuestionFlowStep HotWaterCylinderForwardDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.NumberOfOccupants;
        }

        private QuestionFlowStep NumberOfOccupantsForwardDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.HeatingPattern;
        }

        private QuestionFlowStep HeatingPatternForwardDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint is not null
                ? QuestionFlowStep.AnswerSummary
                : QuestionFlowStep.Temperature;
        }

        private QuestionFlowStep TemperatureForwardDestination()
        {
            return QuestionFlowStep.AnswerSummary;
        }

        private QuestionFlowStep AnswerSummaryForwardDestination(PropertyData propertyData)
        {
            return propertyData.PropertyRecommendations.Any()
                ? QuestionFlowStep.YourRecommendations
                : QuestionFlowStep.NoRecommendations;
        }
        
        private QuestionFlowStep AskForPostcodeSkipDestination()
        {
            return QuestionFlowStep.PropertyType;
        }
    }
}