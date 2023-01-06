using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SeaPublicWebsite.BusinessLogic.Models;
using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace Tests.BusinessLogic.Models;

public class PropertyDataTests
{
    private PropertyData InitializePropertyData()
    {
        return new PropertyData
        {
            Reference = "ABCDEFGH",
            OwnershipStatus = OwnershipStatus.Landlord,
            Country = Country.England,
            Epc = new Epc(),
            SearchForEpc = SearchForEpc.Yes,
            EpcDetailsConfirmed = EpcDetailsConfirmed.Yes,
            PropertyType = PropertyType.Bungalow,
            HouseType = HouseType.Detached,
            BungalowType = BungalowType.Detached,
            FlatType = FlatType.GroundFloor,
            YearBuilt = YearBuilt.Pre1930,
            WallConstruction = WallConstruction.Cavity,
            CavityWallsInsulated = CavityWallsInsulated.All,
            SolidWallsInsulated = SolidWallsInsulated.All,
            FloorConstruction = FloorConstruction.Mix,
            FloorInsulated = FloorInsulated.No,
            RoofConstruction = RoofConstruction.Flat,
            LoftSpace = LoftSpace.No,
            LoftAccess = LoftAccess.No,
            RoofInsulated = RoofInsulated.No,
            HasOutdoorSpace = HasOutdoorSpace.No,
            GlazingType = GlazingType.Both,
            HeatingType = HeatingType.Other,
            OtherHeatingType = OtherHeatingType.Biomass,
            HasHotWaterCylinder = HasHotWaterCylinder.No,
            NumberOfOccupants = 2,
            HeatingPattern = HeatingPattern.Other,
            HoursOfHeatingMorning = 2,
            HoursOfHeatingEvening = 2,
            Temperature = 20,
            UneditedData = new PropertyData(),
            HasSeenRecommendations = false,
            PropertyRecommendations = new List<PropertyRecommendation>
            {
                new()
            }
        };
    }

    public PropertyData PropertyDataWithRecommendations()
    {
        return new PropertyData
        {
            PropertyRecommendations = new List<PropertyRecommendation>
            {
                new()
                {
                    Key = RecommendationKey.InsulateCavityWalls
                },
                new()
                {
                    Key = RecommendationKey.InsulateSolidWalls
                },
                new()
                {
                    Key = RecommendationKey.AddLoftInsulation
                }
            }
        };
    }

    [Test]
    public void CopiesAllAnswers()
    {
        // Arrange
        var propertyData = InitializePropertyData();
        
        // Act
        propertyData.CreateUneditedData();
        
        // Assert
        foreach (var propertyInfo in propertyData.GetType().GetProperties())
        {
            if (propertyInfo.Name.Equals(nameof(PropertyData.Reference)) ||
                propertyInfo.Name.Equals(nameof(PropertyData.Epc)) ||
                propertyInfo.Name.Equals(nameof(PropertyData.UneditedData)) ||
                propertyInfo.Name.Equals(nameof(PropertyData.PropertyRecommendations)))
            {
                continue;
            }
            
            propertyInfo.GetValue(propertyData.UneditedData).Should().NotBeNull();
        }
    }
    
    [Test]
    public void CommitEditsResetsUneditedData()
    {
        // Arrange
        var propertyData = InitializePropertyData();
        
        // Act
        propertyData.CommitEdits();
        
        // Assert
        propertyData.UneditedData.Should().BeNull();
    }
    
    [Test]
    public void CommitEditsResetsPropertyRecommendationsIfThereIsAChange()
    {
        // Arrange
        var propertyData = InitializePropertyData();
        propertyData.CreateUneditedData();
        propertyData.WallConstruction = null;
        
        // Act
        propertyData.CommitEdits();
        
        // Assert
        propertyData.PropertyRecommendations.Should().BeNullOrEmpty();
    }
    
    [Test]
    public void CommitEditsKeepsPropertyRecommendationsIfThereAreNoChange()
    {
        // Arrange
        var propertyData = InitializePropertyData();
        propertyData.CreateUneditedData();
        var previousRecommendations = propertyData.PropertyRecommendations;
        
        // Act
        propertyData.CommitEdits();
        
        // Assert
        propertyData.PropertyRecommendations.Should().Equal(previousRecommendations);
    }

    [Test]
    public void ShowSolidWallsHintIfBuiltBefore1930()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = YearBuilt.Pre1930
        };
        
        // Assert
        propertyData.HintSolidWalls.Should().BeTrue();
    }
    
    [Test]
    public void ShowCavityWallsHintIfBuiltAfter1930()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = YearBuilt.From1930To1966
        };
        
        // Assert
        propertyData.HintSolidWalls.Should().BeFalse();
    }
    
    [Test]
    public void DoNotShowWallsHintIfNoYearBuilt()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = null
        };
        
        // Assert
        propertyData.HintSolidWalls.Should().BeNull();
    }
    
    [Test]
    public void ShowUninsulatedCavityWallsHintIfBuiltBefore1996()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = YearBuilt.From1983To1995
        };
        
        // Assert
        propertyData.HintUninsulatedCavityWalls.Should().BeTrue();
    }
    
    [Test]
    public void ShowInsulatedCavityWallsHintIfBuiltAfter1996()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = YearBuilt.From1996To2011
        };
        
        // Assert
        propertyData.HintUninsulatedCavityWalls.Should().BeFalse();
    }
    
    [Test]
    public void DoNotShowCavityWallsInsulatedHintIfNoYearBuilt()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = null
        };
        
        // Assert
        propertyData.HintUninsulatedCavityWalls.Should().BeNull();
    }
    
    [Test]
    public void ShowSuspendedTimberFloorHintIfBuiltBefore1967()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = YearBuilt.From1930To1966
        };
        
        // Assert
        propertyData.HintSuspendedTimber.Should().BeTrue();
    }
    
    [Test]
    public void ShowSolidConcreteFloorHintIfBuiltAfter1967()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = YearBuilt.From1967To1982
        };
        
        // Assert
        propertyData.HintSuspendedTimber.Should().BeFalse();
    }
    
    [Test]
    public void DoNotShowFloorHintIfNoYearBuilt()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = null
        };
        
        // Assert
        propertyData.HintSuspendedTimber.Should().BeNull();
    }
    
    [Test]
    public void ShowUninsulatedFloorHintIfBuiltBefore1996()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = YearBuilt.From1983To1995
        };
        
        // Assert
        propertyData.HintUninsulatedFloor.Should().BeTrue();
    }
    
    [Test]
    public void ShowInsulatedFloorHintIfBuiltAfter1996()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = YearBuilt.From1996To2011
        };
        
        // Assert
        propertyData.HintUninsulatedFloor.Should().BeFalse();
    }
    
    [Test]
    public void DoNotShowFloorInsulationHintIfNoYearBuilt()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = null
        };
        
        // Assert
        propertyData.HintUninsulatedFloor.Should().BeNull();
    }
    
    [Test]
    public void ShowLoftAccessHintIfPropertyIsHouseOrBungalow()
    {
        // Arrange
        var propertyData1 = new PropertyData
        {
            PropertyType = PropertyType.House
        };
        var propertyData2 = new PropertyData
        {
            PropertyType = PropertyType.Bungalow
        };

        // Assert
        propertyData1.HintHaveLoftAndAccess.Should().BeTrue();
        propertyData2.HintHaveLoftAndAccess.Should().BeTrue();
    }
    
    [Test]
    public void ShowNoLoftAccessHintIfPropertyIsFlat()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            PropertyType = PropertyType.ApartmentFlatOrMaisonette
        };

        // Assert
        propertyData.HintHaveLoftAndAccess.Should().BeFalse();
    }
    
    [Test]
    public void ShowUninsulatedRoofHintIfBuiltBefore2012()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = YearBuilt.From1996To2011
        };
        
        // Assert
        propertyData.HintUninsulatedRoof.Should().BeTrue();
    }
    
    [Test]
    public void ShowInsulatedRoofHintIfBuiltAfter2012()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = YearBuilt.From2012ToPresent
        };
        
        // Assert
        propertyData.HintUninsulatedRoof.Should().BeFalse();
    }
    
    [Test]
    public void DoNotShowRoofInsulationHintIfNoYearBuilt()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = null
        };
        
        // Assert
        propertyData.HintUninsulatedRoof.Should().BeNull();
    }
    
    [Test]
    public void ShowSingleGlazingHintIfBuiltBefore1983()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = YearBuilt.From1967To1982
        };
        
        // Assert
        propertyData.HintSingleGlazing.Should().BeTrue();
    }
    
    [Test]
    public void ShowDoubleOrTripleGlazingHintIfBuiltAfter1983()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = YearBuilt.From1983To1995
        };
        
        // Assert
        propertyData.HintSingleGlazing.Should().BeFalse();
    }
    
    [Test]
    public void DoNotShowGlazingHintIfNoYearBuilt()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            YearBuilt = null
        };
        
        // Assert
        propertyData.HintSingleGlazing.Should().BeNull();
    }
    
    [Test]
    public void ShowHasOutdoorSpaceHintIfPropertyIsHouseOrBungalow()
    {
        // Arrange
        var propertyData1 = new PropertyData
        {
            PropertyType = PropertyType.House
        };
        var propertyData2 = new PropertyData
        {
            PropertyType = PropertyType.Bungalow
        };

        // Assert
        propertyData1.HintHasOutdoorSpace.Should().BeTrue();
        propertyData2.HintHasOutdoorSpace.Should().BeTrue();
    }
    
    [Test]
    public void ShowNoOutdoorSpaceHintIfPropertyIsFlat()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            PropertyType = PropertyType.ApartmentFlatOrMaisonette
        };

        // Assert
        propertyData.HintHasOutdoorSpace.Should().BeFalse();
    }

    [Test]
    public void ShowAlternativeRecommendationRadiatorPanelsIfUserShouldInsulateTheirWalls()
    {
        // Arrange
        var propertyData1 = new PropertyData
        {
            PropertyRecommendations = new List<PropertyRecommendation>
            {
                new()
                {
                    Key = RecommendationKey.InsulateCavityWalls
                }
            }
        };
        var propertyData2 = new PropertyData
        {
            PropertyRecommendations = new List<PropertyRecommendation>
            {
                new()
                {
                    Key = RecommendationKey.InsulateSolidWalls
                }
            }
        };
        
        // Assert
        propertyData1.ShowAltRadiatorPanels.Should().BeTrue();
        propertyData2.ShowAltRadiatorPanels.Should().BeTrue();
    }
    
    [Test]
    public void DoNotShowAlternativeRecommendationRadiatorPanelsIfUserShouldNotInsulateTheirWalls()
    {
        // Arrange
        var propertyData = new PropertyData();
        
        // Assert
        propertyData.ShowAltRadiatorPanels.Should().BeFalse();
    }

    [Test]
    public void ShowAlternativeRecommendationHeatPumpIfUserHeatingTypeIsNotHeatPump()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            HeatingType = HeatingType.GasBoiler
        };
        
        // Assert
        propertyData.ShowAltHeatPump.Should().BeTrue();
    }
    
    [Test]
    public void DoNotShowAlternativeRecommendationHeatPumpIfUserHeatingTypeIsHeatPump()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            HeatingType = HeatingType.HeatPump
        };
        
        // Assert
        propertyData.ShowAltHeatPump.Should().BeFalse();
    }
    
    [Test]
    public void ShowAlternativeRecommendationForFloorIfUserHasUninsulatedFloor()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            FloorInsulated = FloorInsulated.No
        };
        
        // Assert
        propertyData.ShowAltDraughtProofFloors.Should().BeTrue();
    }
    
    [Test]
    public void DoNotShowAlternativeRecommendationForFloorIfUserHasInsulatedFloor()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            FloorInsulated = FloorInsulated.Yes
        };
        
        // Assert
        propertyData.ShowAltDraughtProofFloors.Should().BeFalse();
    }
    
    [Test]
    public void ShowAlternativeRecommendationForGlazingIfUserHasAtLeastOneSingleGlazedWindow()
    {
        // Arrange
        var propertyData1 = new PropertyData
        {
            GlazingType = GlazingType.SingleGlazed
        };
        var propertyData2 = new PropertyData
        {
            GlazingType = GlazingType.Both
        };
        
        // Assert
        propertyData1.ShowAltDraughtProofWindowsAndDoors.Should().BeTrue();
        propertyData2.ShowAltDraughtProofWindowsAndDoors.Should().BeTrue();
    }
    
    [Test]
    public void DoNotShowAlternativeRecommendationForGlazingIfUserHasDoubleOrTripleGlazing()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            GlazingType = GlazingType.DoubleOrTripleGlazed
        };
        
        // Assert
        propertyData.ShowAltDraughtProofWindowsAndDoors.Should().BeFalse();
    }
    
    [Test]
    public void ShowAlternativeRecommendationForLoftIfUserHasAccessibleLoft()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            LoftAccess = LoftAccess.Yes
        };
        
        // Assert
        propertyData.ShowAltDraughtProofLoftAccess.Should().BeTrue();
    }
    
    [Test]
    public void DoNotShowAlternativeRecommendationForLoftIfUserDoesNotHaveAccessibleLoft()
    {
        // Arrange
        var propertyData = new PropertyData
        {
            LoftAccess = LoftAccess.No
        };
        
        // Assert
        propertyData.ShowAltDraughtProofLoftAccess.Should().BeFalse();
    }

    [Test]
    public void CanGetFirstRecommendationKey()
    {
        // Arrange
        var propertyData = PropertyDataWithRecommendations();
        
        // Act
        var key = propertyData.GetFirstRecommendationKey();
        
        // Assert
        key.Should().Be(RecommendationKey.InsulateCavityWalls);
    }
    
    [Test]
    public void CanGetLastRecommendationKey()
    {
        // Arrange
        var propertyData = PropertyDataWithRecommendations();
        
        // Act
        var key = propertyData.GetLastRecommendationKey();
        
        // Assert
        key.Should().Be(RecommendationKey.AddLoftInsulation);
    }
    
    [Test]
    public void CanGetNextRecommendationKey()
    {
        // Arrange
        var propertyData = PropertyDataWithRecommendations();
        
        // Act
        var key = propertyData.GetNextRecommendationKey(RecommendationKey.InsulateCavityWalls);
        
        // Assert
        key.Should().Be(RecommendationKey.InsulateSolidWalls);
    }
    
    [Test]
    public void CanGetPreviousRecommendationKey()
    {
        // Arrange
        var propertyData = PropertyDataWithRecommendations();
        
        // Act
        var key = propertyData.GetPreviousRecommendationKey(RecommendationKey.InsulateSolidWalls);
        
        // Assert
        key.Should().Be(RecommendationKey.InsulateCavityWalls);
    }
    
    [Test]
    public void CanGetRecommendationIndex()
    {
        // Arrange
        var propertyData = PropertyDataWithRecommendations();
        
        // Act
        var index = propertyData.GetRecommendationIndex(RecommendationKey.AddLoftInsulation);
        
        // Assert
        index.Should().Be(2);
    }
}