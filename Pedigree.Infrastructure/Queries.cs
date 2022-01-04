using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Pedigree.Infrastructure
{
	public class Queries
	{
		#region Legacy
		public const string Horse_Heirarchy_Query_Legacy = @"Declare @id int = {0};
    Declare @rootParentOId nvarchar(max), @rootParentName Varchar(max), @age integer, @sex nvarchar(15), @family  nvarchar(15), @country  nvarchar(15);
    Select @rootParentOId = OId, @rootParentName = [Name], @age = Age, @sex = Sex, @family =Family, @country=Country From Horse Where Id = @id
    
    Declare @tblOId as Table
    (
		HorseId integer,
        HorseOId nvarchar(max),
        [Name] nvarchar(max),
		[Age] integer,
		[Sex] nvarchar(15),
		[Family] nvarchar(15),
		[Country] nvarchar(15),
        ParentOId nvarchar(max),
        IsTraversed bit
    )
 
    If(@rootParentOId Is Not NULL)
    Begin
        Insert Into @tblOId(HorseId, HorseOId, [Name], Age, Sex, Family, Country, ParentOId, IsTraversed)
        Values(@id,@rootParentOId, @rootParentName,@age, @sex,@family, @country, NULL, 0)
 
        While((Select Count(1) From @tblOId Where IsTraversed = 0) > 0)
        Begin
            Declare @parentOID nvarchar(max), @parentName Varchar(max);
            Set @parentOID = (Select top 1 HorseOId From @tblOId Where IsTraversed = 0)
            Set @parentName = (Select top 1 [Name] From Horse Where OId = @parentOID)
            --Select @parentOID
 
            Insert Into @tblOId(HorseId, HorseOId,[Name], Age,Sex, Family, Country, ParentOId, IsTraversed)
            Select Id, OId, [Name],Age, Sex, Family, Country, @parentOID, 0
            From Horse
            Where OId In (Select HorseOId From Relationship Where ParentOID = @parentOID)
 
            Update @tblOId
            Set IsTraversed = 1
            Where HorseOId = @parentOID
 
        End
        Select HorseId as Id, HorseOId as OId, [Name],Age,Sex,Family,Country, ParentOId
        From @tblOId
    End
    Else
        Print('No Record Found!')";
		#endregion
		public const string Get_Horse_By_Id = @"
			SELECT
				h.Id, h.OId, h.[Name], h.Age, h.Sex, h.Family, h.MtDNA, CONCAT(hg.Title, '-', ht.[Name]) MtDNATitle, hg.Color MtDNAColor, h.Country, c.COI, c.Pedigcomp, c.GI, c.ZHistoricalBPR, c.ZCurrentBPR, c.Bal, c.AHC, c.Kal, hf.Id FatherId, rf.ParentOId FatherOId, hf.[Name] FatherName, hm.Id MotherId, rm.ParentOId MotherOId, hm.[Name] MotherName
			FROM Horse h
			LEFT JOIN Relationship rf ON h.OId = rf.HorseOId AND rf.ParentType='Father'
			LEFT JOIN Horse hf ON hf.OId = rf.ParentOId
			LEFT JOIN Relationship rm ON h.OId = rm.HorseOId AND rm.ParentType='Mother'
			LEFT JOIN Horse hm ON hm.OId = rm.ParentOId
			LEFT JOIN Coefficient c ON c.HorseOId = h.OId
			LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
			LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
			WHERE h.Id=@Id
		";

		public const string Get_Horse_By_Name = @"
			SELECT
				h.Id, h.OId, h.[Name], h.Age, h.Sex, h.Family, h.MtDNA, CONCAT(hg.Title, '-', ht.[Name]) MtDNATitle, hg.Color MtDNAColor, h.Country, c.COI, c.Pedigcomp, c.GI, c.ZHistoricalBPR, c.ZCurrentBPR, c.Bal, c.AHC, c.Kal, hf.Id FatherId, rf.ParentOId FatherOId, hf.[Name] FatherName, hm.Id MotherId, rm.ParentOId MotherOId, hm.[Name] MotherName
			FROM Horse h
			LEFT JOIN Relationship rf ON h.OId = rf.HorseOId AND rf.ParentType='Father'
			LEFT JOIN Horse hf ON hf.OId = rf.ParentOId
			LEFT JOIN Relationship rm ON h.OId = rm.HorseOId AND rm.ParentType='Mother'
			LEFT JOIN Horse hm ON hm.OId = rm.ParentOId
			LEFT JOIN Coefficient c ON c.HorseOId = h.OId
			LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
			LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
			WHERE h.Name=@Name
		";

		public const string Horse_Heirarchy_Bottom_Up_Query =
            @"
			DECLARE @Id int = {0}; 
			DECLARE @GenerationLevel int = {1}; 
			DECLARE @DepthLevel int = 0;

			DECLARE @Seq int = 0;

			DECLARE @tblOId AS Table
			(
				Id integer IDENTITY(1,1) NOT NULL,
				HorseId integer,
    			HorseOId nvarchar(max),
    			[Name] nvarchar(max),
				Age integer,
				Sex nvarchar(15),
				Family nvarchar(15),
				MtDNATitle nvarchar(25),
				MtDNAColor nvarchar(15),
				Country nvarchar(15),
				Coi float,
				Pedigcomp float,
				Gi float,
				ZHistoricalBPR float,
				ZCurrentBPR float,
				Bal float,
				AHC float,
				Kal float,
				OffspringId integer,
				FatherId integer,
    			FatherOId nvarchar(max),
    			FatherName nvarchar(100),
				MotherId integer,
    			MotherOId nvarchar(max),
    			MotherName nvarchar(100),
    			IsTraversed bit,
				Depth integer,
				SD char,
				Seq integer
			)
 

			IF(@Id IS NOT NULL)
    			BEGIN
        			INSERT INTO @tblOId(HorseId, HorseOId, [Name], Age, Sex, Family, MtDNATitle, MtDNAColor, Country, Coi, Pedigcomp, Gi, ZHistoricalBPR, ZCurrentBPR, Bal, AHC, Kal, OffspringId, FatherId, FatherOId, FatherName, MotherId, MotherOId, MotherName, IsTraversed, Depth, SD, Seq)
        			SELECT h.Id, h.OId, h.[Name], h.Age, h.Sex, h.Family, CONCAT(hg.Title, '-', ht.[Name]), hg.Color, h.Country, c.COI, c.Pedigcomp, c.GI, c.ZHistoricalBPR, c.ZCurrentBPR, c.Bal, c.AHC, c.Kal, 0, hf.Id FatherId, rf.ParentOId FatherOId, hf.[Name] FatherName, hm.Id MotherId, rm.ParentOId MotherOId, hm.[Name] MotherName, 0, @DepthLevel, NULL, @Seq 
					FROM Horse h
					LEFT JOIN Relationship rf ON h.OId = rf.HorseOId AND rf.ParentType='Father'
					LEFT JOIN Horse hf ON hf.OId = rf.ParentOId
					LEFT JOIN Relationship rm ON h.OId = rm.HorseOId AND rm.ParentType='Mother'
					LEFT JOIN Horse hm ON hm.OId = rm.ParentOId
					LEFT JOIN Coefficient c ON c.HorseOId = h.OId
					LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
					LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
					WHERE h.Id = @Id AND h.isDeleted = 0
 		
        			WHILE((SELECT Count(1) FROM @tblOId WHERE IsTraversed = 0) > 0)
        				BEGIN
							DECLARE @tblId integer;
							DECLARE @HorseId integer;
							DECLARE @FatherOId nvarchar(max) ;
							DECLARE @MotherOId nvarchar(max) ;
							DECLARE @SD nvarchar(max) ;
							IF ((SELECT Count(1) FROM @tblOId WHERE IsTraversed = 0 AND Depth = @DepthLevel) > 0)
		    						SELECT TOP 1 @tblId = Id, @HorseId = HorseId, @FatherOId =  FatherOId, @MotherOId =  MotherOId, @DepthLevel = Depth, @SD = SD FROM @tblOId WHERE IsTraversed = 0 AND Depth = @DepthLevel;
							ELSE
								BEGIN
									SET @DepthLevel = @DepthLevel + 1
									SELECT TOP 1 @tblId = Id, @HorseId = HorseId, @FatherOId =  FatherOId, @MotherOId =  MotherOId, @DepthLevel = Depth, @SD = SD FROM @tblOId WHERE IsTraversed = 0 AND Depth = @DepthLevel;
								END

							IF @DepthLevel >= @GenerationLevel
								BREAK;
				
							SET @Seq = @Seq + 1
							IF (@FatherOId IS NOT NULL)
								BEGIN
									IF (@DepthLevel = 0) SET @SD = '{2}';
            						INSERT INTO @tblOId(HorseId, HorseOId, [Name], Age, Sex, Family, MtDNATitle, MtDNAColor, Country, Coi, Pedigcomp, Gi, ZHistoricalBPR, ZCurrentBPR, Bal, AHC, Kal, OffspringId, FatherId, FatherOId, FatherName, MotherId, MotherOId, MotherName, IsTraversed, Depth, SD, Seq)
            						SELECT h.Id, h.OId, h.[Name],h.Age, h.Sex, h.Family, CONCAT(hg.Title, '-', ht.[Name]), hg.Color, h.Country, c.COI, c.Pedigcomp, c.GI, c.ZHistoricalBPR, c.ZCurrentBPR, c.Bal, c.AHC, c.Kal, @HorseId, hf.Id, rf.ParentOId, hf.[Name], hm.Id, rm.ParentOId, hm.[Name], 0, @DepthLevel +  1, @SD, @Seq
            						FROM Horse h
									LEFT JOIN Relationship rf ON rf.HorseOId = @FatherOId AND rf.ParentType='Father'
									LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
									LEFT JOIN Relationship rm ON rm.HorseOId = @FatherOId AND rm.ParentType='Mother'
									LEFT JOIN Horse hm ON hm.OId=rm.ParentOId
									LEFT JOIN Coefficient c ON c.HorseOId=h.OId
									LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
									LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
            						WHERE h.OId = @FatherOId AND h.isDeleted = 0
								END 
			
							SET @Seq = @Seq + 1
							IF (@MotherOId IS NOT NULL)
								BEGIN
            						IF (@DepthLevel = 0) SET @SD = '{3}';
            						INSERT INTO @tblOId(HorseId, HorseOId,[Name], Age,Sex, Family, MtDNATitle, MtDNAColor, Country, Coi, Pedigcomp, Gi, ZHistoricalBPR, ZCurrentBPR, Bal, AHC, Kal, OffspringId, FatherId, FatherOId, FatherName, MotherId, MotherOId, MotherName, IsTraversed, Depth, SD, Seq)
            						SELECT h.Id, h.OId, h.[Name],h.Age, h.Sex, h.Family, CONCAT(hg.Title, '-', ht.[Name]), hg.Color, h.Country, c.COI, c.Pedigcomp, c.GI, c.ZHistoricalBPR, c.ZCurrentBPR, c.Bal, c.AHC, c.Kal, @HorseId, hf.Id, rf.ParentOId, hf.[Name], hm.Id, rm.ParentOId, hm.[Name], 0, @DepthLevel +  1, @SD, @Seq
            						FROM Horse h
									LEFT JOIN Relationship rf ON rf.HorseOId = @MotherOId AND rf.ParentType='Father'
									LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
									LEFT JOIN Relationship rm ON rm.HorseOId = @MotherOId AND rm.ParentType='Mother'
									LEFT JOIN Horse hm ON hm.OId=rm.ParentOId
									LEFT JOIN Coefficient c ON c.HorseOId=h.OId
									LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
									LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
            						WHERE h.OId = @MotherOId AND h.isDeleted = 0
								END
			
							UPDATE @tblOId
            				SET IsTraversed = 1
            				WHERE Id = @tblId
        				END

        				SELECT HorseId AS Id, HorseOId AS OId, [Name], Age, Sex, Family, MtDNATitle, MtDNAColor, Country, Coi, Pedigcomp, Gi, ZHistoricalBPR, ZCurrentBPR, Bal, AHC, Kal, OffspringId, FatherId, FatherOId, FatherName, MotherId, MotherOId, MotherName, Depth, SD, Seq
        				FROM @tblOId 
    			END";

		public const string Get_Horses_Paged_Query =
            @"SELECT 
				h.Id, 
				h.OId, 
				h.[Name], 
				h.Sex, 
				h.Age,
				h.Family, 
				CONCAT(hg.Title, '-', ht.Name) MtDNATitle,
				hg.Color MtDNAColor,
				h.Country,
				dbo.GetBestRaceClass(h.OId) BestRaceClass,
				hf.OId AS FatherOId,
				hf.Name AS FatherName,
				hm.OId AS MotherOId,
				hm.Name AS MotherName 
			FROM Horse h 
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father' 
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId 
			LEFT JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother' 
			LEFT JOIN Horse hm ON hm.OId=rm.ParentOId 
			LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
			LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
			WHERE h.[Name] like '%{0}%' and h.isDeleted = 0 
			ORDER BY {1}
			OFFSET (@PageNumber - 1) * @PageSize ROWS
			FETCH NEXT @PageSize ROWS ONLY;

			SELECT COUNT(Id) FROM Horse h WHERE h.[Name] like '%{0}%' and isDeleted = 0;";

		public const string Search_Exact_Match_Horses_Paged_Query =
			@"SELECT 
				h.Id, 
				h.OId, 
				h.[Name], 
				h.Sex, 
				h.Age,
				h.Family,
				CONCAT(hg.Title, '-', ht.Name) MtDNATitle,
				hg.Color MtDNAColor,
				h.Country,
				dbo.GetBestRaceClass(h.OId) BestRaceClass,
				hf.OId AS FatherOId,
				hf.Name AS FatherName,
				hm.OId AS MotherOId,
				hm.Name AS MotherName
			FROM Horse h  
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father' 
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId 
			LEFT JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother' 
			LEFT JOIN Horse hm ON hm.OId=rm.ParentOId 
			LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
			LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
			WHERE h.[Name] =  '{0}' and h.isDeleted = 0
			ORDER BY {1}
			OFFSET (@PageNumber - 1) * @PageSize ROWS
			FETCH NEXT @PageSize ROWS ONLY;";

		public const string GET_COUNT_HORSES_EXACT_MATCH_Query = @"Select COUNT(Id) from Horse h  where h.[Name] = '{0}' and isDeleted = 0;";

		public const string Search_Horses_With_Gender_Paged_Query =
            @"SELECT 
				h.Id, 
				h.OId, 
				h.[Name], 
				h.Sex, 
				h.Age,
				h.Family, 
				CONCAT(hg.Title, '-', ht.Name) MtDNATitle,
				hg.Color MtDNAColor,
				h.Country,
				dbo.GetBestRaceClass(h.OId) BestRaceClass,
				hf.OId AS FatherOId,
				hf.Name AS FatherName,
				hm.OId AS MotherOId,
				hm.Name AS MotherName 
			FROM Horse h 
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father' 
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId 
			LEFT JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother' 
			LEFT JOIN Horse hm ON hm.OId=rm.ParentOId 
			LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
			LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
			WHERE h.[Name] LIKE '{0}%' AND h.isDeleted = 0 AND h.Sex = '{1}'
			ORDER BY {2}
			OFFSET (@PageNumber-1) * @PageSize ROWS
			FETCH NEXT @PageSize ROWS ONLY;
			
			SELECT COUNT(Id) FROM Horse h WHERE h.[Name] LIKE '{0}%' AND h.isDeleted = 0 AND h.Sex = '{1}'";

		public const string Check_Dupliacte_Horse_By_Params = @"Select Id from Horse where Name = @Name and Age = @Age and Sex = @Sex and Country = @Country and isDeleted=0;";

		public const string Get_Female_Tail_By_Horse =
			@"
				Declare @id int = {0}; 
				Declare @DepthLevel int = 0;

    			Declare @tblOId as Table
    			(
					HorseId integer,
        			HorseOId nvarchar(100),
        			[Name] nvarchar(200),
					[Age] integer,
					[Sex] nvarchar(15),
					[Family] nvarchar(15),
					MtDNATitle nvarchar(25),
					MtDNAColor nvarchar(10),
					MtDNAFlag integer,
					[Country] nvarchar(15),
        			FatherOId nvarchar(100),
					FatherName nvarchar(100),
					MotherOId nvarchar(100),
					MotherName nvarchar(100),
        			IsTraversed bit,
					Depth integer
    			)
 
    			If(@id Is Not NULL)
    			Begin
        			Insert Into @tblOId(HorseId, HorseOId, [Name], Age, Sex, Family, MtDNATitle, MtDNAColor, MtDNAFlag, Country, FatherOId, FatherName, MotherOId, MotherName, IsTraversed, Depth)
        			Select h.Id, h.OId, h.[Name], h.Age, h.Sex, h.Family, CONCAT(hg.Title, '-', ht.[Name]), hg.Color, h.MtDNAFlag, h.Country, rf.ParentOId FatherOId, hf.[Name] FatherName, rm.ParentOId MotherOId, hm.[Name] MotherName, 0, @DepthLevel from Horse h
					join Relationship rm on h.OId = rm.HorseOId and rm.ParentType = 'Mother'
					left join Horse hm on hm.OId=rm.ParentOId
					left join Relationship rf on h.OId=rf.HorseOId and rf.ParentType='Father'
					left join Horse hf on hf.OId=rf.ParentOId
					left join HaploType ht on ht.Id=h.MtDNA
					left join HaploGroup hg on hg.Id=ht.GroupId
					where h.Id = @id and h.isDeleted = 0 
 		
        			While((Select Count(1) From @tblOId Where IsTraversed = 0) > 0)
        			Begin
						Declare @MotherOId nvarchar(100) ;
						Select top 1 @MotherOId =  MotherOId, @DepthLevel = Depth From @tblOId Where IsTraversed = 0
			
            			Insert Into @tblOId(HorseId, HorseOId,[Name], Age, Sex, Family, MtDNATitle, MtDNAColor, MtDNAFlag, Country, FatherOId, FatherName, MotherOId, MotherName, IsTraversed, Depth)
            			Select hr.Id, hr.OId, hr.[Name],hr.Age, hr.Sex, hr.Family, CONCAT(hg.Title, '-', ht.[Name]), hg.Color, hr.MtDNAFlag, hr.Country, rf.ParentOId FatherOId, hf.[Name] FatherName, rm.ParentOId MotherOId, hm.[Name] MotherName, 0, @DepthLevel +  1
            			From Horse hr
						left join Relationship rm on rm.HorseOId = @MotherOId and rm.ParentType = 'Mother'
						left join Horse hm on hm.OId=rm.ParentOId
						left join Relationship rf on hr.OId=rf.HorseOId and rf.ParentType='Father'
						left join Horse hf on hf.OId=rf.ParentOId
						left join HaploType ht on ht.Id=hr.MtDNA
						left join HaploGroup hg on hg.Id=ht.GroupId
            			--Where OId In (Select HorseOId From Relationship Where ParentOId = @parentOID)
						where hr.OId = @MotherOId and hr.isDeleted = 0 

						if ((Select Count(1) From Relationship r Where r.HorseOId = @MotherOId) < 1)
							break;

            			Update @tblOId
            			Set IsTraversed = 1
            			Where MotherOId = @MotherOId and Depth = @DepthLevel
 
        			End
        			Select HorseId as Id, HorseOId as OId, [Name], Age, Sex, Family, MtDNATitle, MtDNAColor, MtDNAFlag, Country, FatherOId, FatherName, MotherOId, MotherName, Depth, IsTraversed
        			From @tblOId 
    			End
			";

		public const string Get_Sibilings_Horse_From_Same_Parent_Sex = @"
			Declare @parentId nvarchar(50);
			set @parentId = (select ParentOId from Relationship r join Horse h on h.OId = r.HorseOId and r.ParentType=@ParentType where h.Id = @Id);
			print @parentId;
			select h.Id, 
				h.OId, 
				h.[Name], 
				h.Sex, 
				h.Age,
				h.Family, 
				h.Country,
				concat(hg.Title, '-', ht.[Name]) MtDNATitle,
				hg.Color MtDNAColor,
				dbo.GetBestRaceClass(h.OId) BestRaceClass,				
				hf.OId AS FatherOId,
				hf.Name AS FatherName,
				hm.OId AS MotherOId,
				hm.Name AS MotherName 
			from Horse h 
			join Relationship r on h.OId = r.HorseOId 
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father' 
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId 
			LEFT JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother' 
			LEFT JOIN Horse hm ON hm.OId=rm.ParentOId 
			LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
			LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId

			where r.ParentOId = @parentId and h.Id <> @Id order by Age desc;";

		public const string Get_Offspring_By_Horse = @"
			select DISTINCT
				h.*,
				concat(hg.Title, '-', ht.Name) MtDNATitle,
				hg.Color MtDNAColor,
				dbo.GetBestRaceClass(h.OId) BestRaceClass,
				hf.Id FatherId,
				hf.OId FatherOId,
				hf.[Name] FatherName,
				hm.Id MotherId,
				hm.OId MotherOId,
				hm.[Name] MotherName,
				c.CurrentBPR,
				c.ZCurrentBPR
			from Horse h 
			join Relationship r on h.OId = r.HorseOId
			LEFT JOIN Coefficient c ON c.HorseOId=h.OId
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father' 
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId 
			LEFT JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother' 
			LEFT JOIN Horse hm ON hm.OId=rm.ParentOId 
			left join HaploType ht on ht.Id=h.MtDNA
			left join HaploGroup hg on hg.Id=ht.GroupId
			LEFT JOIN Position p ON p.HorseOId=h.OId
			where r.ParentOId = (select OId from Horse where id = @Id) order by Age desc";

		public static string Get_Positions_Paged_Query (int raceId, string q)
        {
			var query = @"SELECT 
				p.Id,
				p.RaceId,
				p.HorseOId,
				p.Place,
				h.Id HorseId,
				h.Name HorseName,
				h.Age HorseAge,
				h.Sex HorseSex,
				h.Country HorseCountry,
				h.Family HorseFamily,
				hf.Name HorseFather,
				hm.Name HorseMother
			FROM 
				Position p 
			LEFT JOIN 
				Horse h ON h.OId=p.HorseOId
			LEFT JOIN 
				Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father' 
			LEFT JOIN 
				Horse hf ON hf.OId=rf.ParentOId 
			LEFT JOIN 
				Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother' 
			LEFT JOIN	
				Horse hm ON hm.OId=rm.ParentOId 
			WHERE {0}
			ORDER BY p.Place ASC
			OFFSET (@PageNumber - 1) * @PageSize ROWS
			FETCH NEXT @PageSize ROWS ONLY;

			SELECT 
				COUNT(*) 
			FROM 
				Position p 
			LEFT JOIN 
				Horse h ON h.OId=p.HorseOId
			LEFT JOIN 
				Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father' 
			LEFT JOIN 
				Horse hf ON hf.OId=rf.ParentOId 
			LEFT JOIN 
				Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother' 
			LEFT JOIN 
				Horse hm ON hm.OId=rm.ParentOId 
			WHERE {0}";

			string where = $"p.RaceId = {raceId}";
			if (q != null) where += $" AND (h.Name like '%{q}%' OR hf.Name like '%{q}%' OR hm.Name like '%{q}%')";

			return string.Format(query, where);
        }

		public const string Get_All_Races = @"
			SELECT 
				r.*, 
				w.Value [Weight],
				(SELECT COUNT(*) FROM Position WHERE RaceId=r.Id) Rnrs
			FROM Race r
			LEFT JOIN [Weight] AS w ON w.Country = r.Country AND w.Distance = r.Distance AND w.Surface = r.Surface AND w.[Type] = r.[Type] AND w.[Status] = r.[Status];
		";

        public const string Get_Hist_Races = @"
			SELECT 
				r.*, 
				w.Value [Weight],
				(SELECT COUNT(*) FROM Position WHERE RaceId=r.Id) Rnrs
			FROM Race r
			LEFT JOIN [Weight] AS w ON w.Country = r.Country AND w.Distance = r.Distance AND w.Surface = r.Surface AND w.[Type] = r.[Type] AND w.[Status] = r.[Status]
			WHERE DATEDIFF(MONTH, r.[Date], CURRENT_TIMESTAMP) > 48;
		";

        public static string Get_Races_Paged_Query (string q, string sorting) {
			var query = @"
			SELECT 
				r.Id, 
				r.[Name],
				r.[Date],
				r.Country,
				r.Distance,
				r.Surface,
				r.[Type],
				r.[Status],
				w.[Value] AS [Weight],
				(SELECT COUNT(*) FROM Position WHERE RaceId=r.Id) Rnrs,
				(SELECT SUM(CurrentBPR)/COUNT(*) FROM Coefficient WHERE HorseOId IN (SELECT HorseOId FROM Position WHERE RaceId=r.Id)) BPR
			FROM Race AS r
			LEFT JOIN Weight AS w ON w.Country = r.Country AND w.Distance = r.Distance AND w.Surface = r.Surface AND w.[Type] = r.[Type] AND w.[Status] = r.[Status]			
			WHERE {0}
			ORDER BY {1}
			OFFSET (@PageNumber - 1) * @PageSize ROWS
			FETCH NEXT @PageSize ROWS ONLY;

			SELECT COUNT(Id) FROM Race r WHERE {0};";

			string where = "1=1";

			try
			{
				var filter = JsonSerializer.Deserialize<RaceQueryFilterObj>(q);

				if (filter.Name != null) where += $" AND [Name] LIKE '%{filter.Name}%'";
				if (filter.Year > 0) where += $" AND [Date] BETWEEN '{filter.Year}-01-01' AND '{filter.Year}-12-31'";
				if (filter.Country != null && filter.Country.Length > 0) where += $" AND r.Country IN ('{string.Join("','", filter.Country)}')";
				if (filter.Distance != null && filter.Distance.Length > 0) where += $" AND r.Distance IN ('{string.Join("','", filter.Distance)}')";
				if (filter.Surface != null && filter.Surface.Length > 0) where += $" AND r.Surface IN ('{string.Join("','", filter.Surface)}')";
				if (filter.Type != null && filter.Type.Length > 0) where += $" AND r.Type IN ('{string.Join("','", filter.Type)}')";
				if (filter.Status != null && filter.Status.Length > 0) where += $" AND r.Status IN ('{string.Join("','", filter.Status)}')";
			}
			catch(Exception e)
            {
				Console.WriteLine(e.StackTrace);
            }
			
			return string.Format(query, where, sorting);
		}

		public static string Get_Weights_Paged_Query(string q, string sorting)
		{
			var query = @"SELECT 
				Id, 
				Country,
				Distance,
				Surface,
				Type,
				Status,
				Value
			FROM Weight
			WHERE {0}
			ORDER BY {1}
			OFFSET (@PageNumber - 1) * @PageSize ROWS
			FETCH NEXT @PageSize ROWS ONLY;

			SELECT COUNT(Id) FROM Weight WHERE {0};";

			string where = "1=1";

			try
			{
				var filter = JsonSerializer.Deserialize<RaceQueryFilterObj>(q);

				if (filter.Country != null && filter.Country.Length > 0) where += $" AND Country IN ('{string.Join("','", filter.Country)}')";
				if (filter.Distance != null && filter.Distance.Length > 0) where += $" AND Distance IN ('{string.Join("','", filter.Distance)}')";
				if (filter.Surface != null && filter.Surface.Length > 0) where += $" AND Surface IN ('{string.Join("','", filter.Surface)}')";
				if (filter.Type != null && filter.Type.Length > 0) where += $" AND Type IN ('{string.Join("','", filter.Type)}')";
				if (filter.Status != null && filter.Status.Length > 0) where += $" AND Status IN ('{string.Join("','", filter.Status)}')";
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}

			return string.Format(query, where, sorting);
		}

		public const string Get_Unique_Ancestors_By_Horse = @"	
			Declare @id int = {0}; 
			DECLARE @MaxGen int = {1}; 
			DECLARE @DepthLevel int = 0;

    		Declare @tblOId as Table
    		(
				Id integer IDENTITY(1,1) NOT NULL,
				HorseId integer,
        		HorseOId nvarchar(100),
        		[Name] nvarchar(200),
				[Age] integer,
				[Sex] nvarchar(15),
				[Family] nvarchar(15),
				MtDNA integer,
				MtDNATitle nvarchar(25),
				MtDNAColor nvarchar(25),
				[Country] nvarchar(15),
				BestRaceClass nvarchar(10),
        		FatherOId nvarchar(100),
				FatherName nvarchar(100),
				MotherOId nvarchar(100),
				MotherName nvarchar(100),
				HistoricalBPR float,
				ZHistoricalBPR float,
        		IsTraversed bit,
				Depth integer
    		)
 
    		If(@id Is Not NULL)
    		Begin
        		Insert Into @tblOId(HorseId, HorseOId, [Name], Age, Sex, Family, MtDNA, MtDNATitle, MtDNAColor, Country, BestRaceClass, FatherOId, FatherName, MotherOId, MotherName, HistoricalBPR, ZHistoricalBPR, IsTraversed, Depth)
        		Select h.Id, h.OId, h.[Name], h.Age, h.Sex, h.Family, h.MtDNA, concat(hg.Title,'-',ht.[Name]), hg.Color, h.Country, dbo.GetBestRaceClass(h.OId), rf.ParentOId FatherOId, hf.[Name] FatherName, rm.ParentOId MotherOId, hm.[Name] MotherName, c.HistoricalBPR, c.ZHistoricalBPR, 0, 0 
				from Horse h
				left join Coefficient c ON c.HorseOId=h.OId
				left join Relationship rf on h.OId=rf.HorseOId and rf.ParentType='Father'
				left join Horse hf on hf.OId=rf.ParentOId
				left join Relationship rm on h.OId = rm.HorseOId and rm.ParentType = 'Mother'
				left join Horse hm on hm.OId=rm.ParentOId
				left join HaploType ht on ht.Id=h.MtDNA
				left join HaploGroup hg on hg.Id=ht.GroupId
				where h.Id = @id and h.isDeleted = 0 
 		
        		While((Select Count(1) From @tblOId Where IsTraversed = 0) > 0)
        		Begin
					Declare @tblId int;
					Declare @FatherOId nvarchar(100) ;
					Declare @MotherOId nvarchar(100) ;
			
					IF ((SELECT Count(1) FROM @tblOId WHERE IsTraversed = 0 AND Depth = @DepthLevel) > 0)
		    				SELECT TOP 1 @tblId = Id, @FatherOId =  FatherOId, @MotherOId =  MotherOId, @DepthLevel = Depth FROM @tblOId WHERE IsTraversed = 0 AND Depth = @DepthLevel;
					ELSE
						BEGIN
							SET @DepthLevel = @DepthLevel + 1
							SELECT TOP 1 @tblId = Id, @FatherOId =  FatherOId, @MotherOId =  MotherOId, @DepthLevel = Depth FROM @tblOId WHERE IsTraversed = 0 AND Depth = @DepthLevel;
						END
			
			
					if (@DepthLevel >= @MaxGen)
						break;

					-- Father Side
            		Insert Into @tblOId(HorseId, HorseOId,[Name], Age, Sex, Family, MtDNA, MtDNATitle, MtDNAColor, Country, BestRaceClass, FatherOId, FatherName, MotherOId, MotherName, HistoricalBPR, ZHistoricalBPR, IsTraversed, Depth)
            		Select hr.Id, hr.OId, hr.[Name],hr.Age, hr.Sex, hr.Family, hr.MtDNA, concat(hg.Title,'-',ht.[Name]), hg.Color, hr.Country, dbo.GetBestRaceClass(hr.OId), rf.ParentOId FatherOId, hf.[Name] FatherName, rm.ParentOId MotherOId, hm.[Name] MotherName, c.HistoricalBPR, c.ZHistoricalBPR, 0, @DepthLevel +  1
            		From Horse hr
					left join Coefficient c ON c.HorseOId=hr.OId
					left join Relationship rf on rf.HorseOId = @FatherOId and rf.ParentType='Father'
					left join Horse hf on hf.OId=rf.ParentOId
            		left join Relationship rm on rm.HorseOId = @FatherOId and rm.ParentType = 'Mother'
					left join Horse hm on hm.OId=rm.ParentOId
					left join HaploType ht on ht.Id=hr.MtDNA
					left join HaploGroup hg on hg.Id=ht.GroupId
					where hr.OId = @FatherOId and hr.isDeleted = 0 and (select count(1) from @tblOId where HorseOId=@FatherOId) = 0

					-- Mother Side
            		Insert Into @tblOId(HorseId, HorseOId,[Name], Age, Sex, Family, MtDNA, MtDNATitle, MtDNAColor, Country, BestRaceClass, FatherOId, FatherName, MotherOId, MotherName, HistoricalBPR, ZHistoricalBPR, IsTraversed, Depth)
            		Select hr.Id, hr.OId, hr.[Name],hr.Age, hr.Sex, hr.Family, hr.MtDNA, concat(hg.Title,'-',ht.[Name]), hg.Color, hr.Country, dbo.GetBestRaceClass(hr.OId), rf.ParentOId FatherOId, hf.[Name] FatherName, rm.ParentOId MotherOId, hm.[Name] MotherName, c.HistoricalBPR, c.ZHistoricalBPR, 0, @DepthLevel +  1
            		From Horse hr
					left join Coefficient c ON c.HorseOId=hr.OId
					left join Relationship rf on rf.HorseOId = @MotherOId and rf.ParentType='Father'
					left join Horse hf on hf.OId=rf.ParentOId
            		left join Relationship rm on rm.HorseOId = @MotherOId and rm.ParentType = 'Mother'
					left join Horse hm on hm.OId=rm.ParentOId
					left join HaploType ht on ht.Id=hr.MtDNA
					left join HaploGroup hg on hg.Id=ht.GroupId
					where hr.OId = @MotherOId and hr.isDeleted = 0 and (select count(1) from @tblOId where HorseOId=@MotherOId) = 0 


            		Update @tblOId
            		Set IsTraversed = 1
            		Where Id = @tblId
 
        		End

        		Select Id as _Id, HorseId as Id, HorseOId as OId, [Name], Age, Sex, Family, MtDNA, MtDNATitle, MtDNAColor, Country, BestRaceClass, FatherName, FatherOId, MotherName, MotherOId, HistoricalBPR, ZHistoricalBPR, Depth
        		From @tblOId
				--Where Depth > 0
				Order By _Id ASC
    		End";

		public const string Get_Inbreedings_By_Horse = @"SELECT * FROM Inbreed WHERE InbreedOId='{0}' AND OId IN ('{1}')";

		public const string Get_Inbreeding_Horses_Paged_Query = @"
			DECLARE @HorseOId nvarchar(100) = '{0}';

			DELETE FROM Inbreed WHERE DATEDIFF(minute, CreatedAt, CURRENT_TIMESTAMP) > 30
			
			IF (SELECT COUNT(*) FROM Inbreed WHERE InbreedOId=@HorseOId) = 0
				BEGIN
					---- 1 ----
					INSERT INTO Inbreed(InbreedOId, OId, SD, Depth)
					SELECT @HorseOId, r1.HorseOId, CASE WHEN r1.ParentType='Father' THEN 'S' ELSE 'D' END, 1					
					FROM Horse h 
					JOIN Relationship r1 ON r1.HorseOId=h.OId
					LEFT JOIN Relationship r2 ON r1.ParentOId = r2.HorseOId
					LEFT JOIN Relationship r3 ON r2.ParentOId = r3.HorseOId 
					LEFT JOIN Relationship r4 ON r3.ParentOId = r4.HorseOId 
					LEFT JOIN Relationship r5 ON r4.ParentOId = r5.HorseOId
					LEFT JOIN Relationship r6 ON r5.ParentOId = r6.HorseOId
					LEFT JOIN Relationship r7 ON r6.ParentOId = r7.HorseOId 
					LEFT JOIN Relationship r8 ON r7.ParentOId = r8.HorseOId
					LEFT JOIN Relationship r9 ON r8.ParentOId = r9.HorseOId
					LEFT JOIN Relationship r10 ON r9.ParentOId = r10.HorseOId 
					WHERE r1.ParentOId = @HorseOId
					GROUP BY r1.HorseOId, r1.ParentType

					---- 2 ----
					INSERT INTO Inbreed(InbreedOId, OId, SD, Depth)
					SELECT @HorseOId, r1.HorseOId, CASE WHEN r1.ParentType='Father' THEN 'S' ELSE 'D' END, 2
					FROM Horse h 
					JOIN Relationship r1 ON r1.HorseOId=h.OId
					LEFT JOIN Relationship r2 ON r1.ParentOId = r2.HorseOId
					LEFT JOIN Relationship r3 ON r2.ParentOId = r3.HorseOId 
					LEFT JOIN Relationship r4 ON r3.ParentOId = r4.HorseOId 
					LEFT JOIN Relationship r5 ON r4.ParentOId = r5.HorseOId
					LEFT JOIN Relationship r6 ON r5.ParentOId = r6.HorseOId
					LEFT JOIN Relationship r7 ON r6.ParentOId = r7.HorseOId 
					LEFT JOIN Relationship r8 ON r7.ParentOId = r8.HorseOId
					LEFT JOIN Relationship r9 ON r8.ParentOId = r9.HorseOId
					LEFT JOIN Relationship r10 ON r9.ParentOId = r10.HorseOId 
					WHERE r2.ParentOId = @HorseOId
					GROUP BY r1.HorseOId, r1.ParentType

					---- 3 ----
					INSERT INTO Inbreed(InbreedOId, OId, SD, Depth)
					SELECT @HorseOId, r1.HorseOId, CASE WHEN r1.ParentType='Father' THEN 'S' ELSE 'D' END, 3
					FROM Horse h 
					JOIN Relationship r1 ON r1.HorseOId=h.OId
					LEFT JOIN Relationship r2 ON r1.ParentOId = r2.HorseOId
					LEFT JOIN Relationship r3 ON r2.ParentOId = r3.HorseOId 
					LEFT JOIN Relationship r4 ON r3.ParentOId = r4.HorseOId 
					LEFT JOIN Relationship r5 ON r4.ParentOId = r5.HorseOId
					LEFT JOIN Relationship r6 ON r5.ParentOId = r6.HorseOId
					LEFT JOIN Relationship r7 ON r6.ParentOId = r7.HorseOId 
					LEFT JOIN Relationship r8 ON r7.ParentOId = r8.HorseOId
					LEFT JOIN Relationship r9 ON r8.ParentOId = r9.HorseOId
					LEFT JOIN Relationship r10 ON r9.ParentOId = r10.HorseOId 
					WHERE r3.ParentOId = @HorseOId
					GROUP BY r1.HorseOId, r1.ParentType

					---- 4 ----
					INSERT INTO Inbreed(InbreedOId, OId, SD, Depth)
					SELECT @HorseOId, r1.HorseOId, CASE WHEN r1.ParentType='Father' THEN 'S' ELSE 'D' END, 4
					FROM Horse h 
					JOIN Relationship r1 ON r1.HorseOId=h.OId
					LEFT JOIN Relationship r2 ON r1.ParentOId = r2.HorseOId
					LEFT JOIN Relationship r3 ON r2.ParentOId = r3.HorseOId 
					LEFT JOIN Relationship r4 ON r3.ParentOId = r4.HorseOId 
					LEFT JOIN Relationship r5 ON r4.ParentOId = r5.HorseOId
					LEFT JOIN Relationship r6 ON r5.ParentOId = r6.HorseOId
					LEFT JOIN Relationship r7 ON r6.ParentOId = r7.HorseOId 
					LEFT JOIN Relationship r8 ON r7.ParentOId = r8.HorseOId
					LEFT JOIN Relationship r9 ON r8.ParentOId = r9.HorseOId
					LEFT JOIN Relationship r10 ON r9.ParentOId = r10.HorseOId 
					WHERE r4.ParentOId = @HorseOId
					GROUP BY r1.HorseOId, r1.ParentType

					---- 5 ----
					INSERT INTO Inbreed(InbreedOId, OId, SD, Depth)
					SELECT @HorseOId, r1.HorseOId, CASE WHEN r1.ParentType='Father' THEN 'S' ELSE 'D' END, 5
					FROM Horse h 
					JOIN Relationship r1 ON r1.HorseOId=h.OId
					LEFT JOIN Relationship r2 ON r1.ParentOId = r2.HorseOId
					LEFT JOIN Relationship r3 ON r2.ParentOId = r3.HorseOId 
					LEFT JOIN Relationship r4 ON r3.ParentOId = r4.HorseOId 
					LEFT JOIN Relationship r5 ON r4.ParentOId = r5.HorseOId
					LEFT JOIN Relationship r6 ON r5.ParentOId = r6.HorseOId
					LEFT JOIN Relationship r7 ON r6.ParentOId = r7.HorseOId 
					LEFT JOIN Relationship r8 ON r7.ParentOId = r8.HorseOId
					LEFT JOIN Relationship r9 ON r8.ParentOId = r9.HorseOId
					LEFT JOIN Relationship r10 ON r9.ParentOId = r10.HorseOId 
					WHERE r5.ParentOId = @HorseOId
					GROUP BY r1.HorseOId, r1.ParentType

					---- 6 ----
					INSERT INTO Inbreed(InbreedOId, OId, SD, Depth)
					SELECT @HorseOId, r1.HorseOId, CASE WHEN r1.ParentType='Father' THEN 'S' ELSE 'D' END, 6
					FROM Horse h 
					JOIN Relationship r1 ON r1.HorseOId=h.OId
					LEFT JOIN Relationship r2 ON r1.ParentOId = r2.HorseOId
					LEFT JOIN Relationship r3 ON r2.ParentOId = r3.HorseOId 
					LEFT JOIN Relationship r4 ON r3.ParentOId = r4.HorseOId 
					LEFT JOIN Relationship r5 ON r4.ParentOId = r5.HorseOId
					LEFT JOIN Relationship r6 ON r5.ParentOId = r6.HorseOId
					LEFT JOIN Relationship r7 ON r6.ParentOId = r7.HorseOId 
					LEFT JOIN Relationship r8 ON r7.ParentOId = r8.HorseOId
					LEFT JOIN Relationship r9 ON r8.ParentOId = r9.HorseOId
					LEFT JOIN Relationship r10 ON r9.ParentOId = r10.HorseOId 
					WHERE r6.ParentOId = @HorseOId
					GROUP BY r1.HorseOId, r1.ParentType

					---- 7 ----
					INSERT INTO Inbreed(InbreedOId, OId, SD, Depth)
					SELECT @HorseOId, r1.HorseOId, CASE WHEN r1.ParentType='Father' THEN 'S' ELSE 'D' END, 7
					FROM Horse h 
					JOIN Relationship r1 ON r1.HorseOId=h.OId
					LEFT JOIN Relationship r2 ON r1.ParentOId = r2.HorseOId
					LEFT JOIN Relationship r3 ON r2.ParentOId = r3.HorseOId 
					LEFT JOIN Relationship r4 ON r3.ParentOId = r4.HorseOId 
					LEFT JOIN Relationship r5 ON r4.ParentOId = r5.HorseOId
					LEFT JOIN Relationship r6 ON r5.ParentOId = r6.HorseOId
					LEFT JOIN Relationship r7 ON r6.ParentOId = r7.HorseOId 
					LEFT JOIN Relationship r8 ON r7.ParentOId = r8.HorseOId
					LEFT JOIN Relationship r9 ON r8.ParentOId = r9.HorseOId
					LEFT JOIN Relationship r10 ON r9.ParentOId = r10.HorseOId 
					WHERE r7.ParentOId = @HorseOId
					GROUP BY r1.HorseOId, r1.ParentType

					---- 8 ----
					INSERT INTO Inbreed(InbreedOId, OId, SD, Depth)
					SELECT @HorseOId, r1.HorseOId, CASE WHEN r1.ParentType='Father' THEN 'S' ELSE 'D' END, 8
					FROM Horse h 
					JOIN Relationship r1 ON r1.HorseOId=h.OId
					LEFT JOIN Relationship r2 ON r1.ParentOId = r2.HorseOId
					LEFT JOIN Relationship r3 ON r2.ParentOId = r3.HorseOId 
					LEFT JOIN Relationship r4 ON r3.ParentOId = r4.HorseOId 
					LEFT JOIN Relationship r5 ON r4.ParentOId = r5.HorseOId
					LEFT JOIN Relationship r6 ON r5.ParentOId = r6.HorseOId
					LEFT JOIN Relationship r7 ON r6.ParentOId = r7.HorseOId 
					LEFT JOIN Relationship r8 ON r7.ParentOId = r8.HorseOId
					LEFT JOIN Relationship r9 ON r8.ParentOId = r9.HorseOId
					LEFT JOIN Relationship r10 ON r9.ParentOId = r10.HorseOId 
					WHERE r8.ParentOId = @HorseOId
					GROUP BY r1.HorseOId, r1.ParentType

					---- 9 ----
					INSERT INTO Inbreed(InbreedOId, OId, SD, Depth)
					SELECT @HorseOId, r1.HorseOId, CASE WHEN r1.ParentType='Father' THEN 'S' ELSE 'D' END, 9
					FROM Horse h 
					JOIN Relationship r1 ON r1.HorseOId=h.OId
					LEFT JOIN Relationship r2 ON r1.ParentOId = r2.HorseOId
					LEFT JOIN Relationship r3 ON r2.ParentOId = r3.HorseOId 
					LEFT JOIN Relationship r4 ON r3.ParentOId = r4.HorseOId 
					LEFT JOIN Relationship r5 ON r4.ParentOId = r5.HorseOId
					LEFT JOIN Relationship r6 ON r5.ParentOId = r6.HorseOId
					LEFT JOIN Relationship r7 ON r6.ParentOId = r7.HorseOId 
					LEFT JOIN Relationship r8 ON r7.ParentOId = r8.HorseOId
					LEFT JOIN Relationship r9 ON r8.ParentOId = r9.HorseOId
					LEFT JOIN Relationship r10 ON r9.ParentOId = r10.HorseOId 
					WHERE r9.ParentOId = @HorseOId
					GROUP BY r1.HorseOId, r1.ParentType

					---- 10 ----
					INSERT INTO Inbreed(InbreedOId, OId, SD, Depth)
					SELECT @HorseOId, r1.HorseOId, CASE WHEN r1.ParentType='Father' THEN 'S' ELSE 'D' END, 10
					FROM Horse h 
					JOIN Relationship r1 ON r1.HorseOId=h.OId
					LEFT JOIN Relationship r2 ON r1.ParentOId = r2.HorseOId
					LEFT JOIN Relationship r3 ON r2.ParentOId = r3.HorseOId 
					LEFT JOIN Relationship r4 ON r3.ParentOId = r4.HorseOId 
					LEFT JOIN Relationship r5 ON r4.ParentOId = r5.HorseOId
					LEFT JOIN Relationship r6 ON r5.ParentOId = r6.HorseOId
					LEFT JOIN Relationship r7 ON r6.ParentOId = r7.HorseOId 
					LEFT JOIN Relationship r8 ON r7.ParentOId = r8.HorseOId
					LEFT JOIN Relationship r9 ON r8.ParentOId = r9.HorseOId
					LEFT JOIN Relationship r10 ON r9.ParentOId = r10.HorseOId 
					WHERE r10.ParentOId = @HorseOId
					GROUP BY r1.HorseOId, r1.ParentType
				END

			SELECT 
				h.Id, h.OId, h.[Name], h.Age, h.Sex, h.Country, h.Family
			FROM Horse h 
			WHERE h.OId IN (SELECT OId FROM Inbreed WHERE InbreedOId=@HorseOId GROUP BY OId HAVING COUNT(*) > 1)
			ORDER BY {1}
			OFFSET (@PageNumber - 1) * @PageSize ROWS
			FETCH NEXT @PageSize ROWS ONLY;

			SELECT COUNT(*) FROM (SELECT OId FROM Inbreed WHERE InbreedOId=@HorseOId GROUP BY OId HAVING COUNT(*) > 1) F;
		";
		public const string Incompleted_Pedigree_Horses_Paged_Query = @"
			SELECT 
				h.Id, 
				h.OId, 
				h.[Name], 
				h.Sex, 
				h.Age,
				h.Family, 
				h.Country,
				hf.OId AS FatherOId,
				hf.Name AS FatherName,
				hm.OId AS MotherOId,
				hm.Name AS MotherName 
			FROM Horse h 
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father' 
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId 
			LEFT JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother' 
			LEFT JOIN Horse hm ON hm.OId=rm.ParentOId 
			LEFT JOIN Coefficient c ON c.HorseOId=h.OId
			WHERE h.Age = {0} AND (c.Pedigcomp IS NULL OR c.Pedigcomp < 100) AND h.isDeleted = 0 
			ORDER BY {1}
			OFFSET (@PageNumber - 1) * @PageSize ROWS
			FETCH NEXT @PageSize ROWS ONLY;

			SELECT COUNT(h.Id) FROM Horse h LEFT JOIN Coefficient c ON c.HorseOId=h.OId WHERE h.Age = {0} and c.Pedigcomp < 100 and h.isDeleted = 0;
		";

		public const string Get_Twins_Dam_List_Query = @"
			DECLARE @tblTwin AS Table
			(
				OId nvarchar(100),
				TwinYear integer,
				Twins integer
			)

			INSERT INTO @tblTwin (OId, TwinYear, Twins)
			SELECT hm.OId, h.Age TwinYear, COUNT(*) Twins
			FROM Relationship r
			JOIN Horse h ON h.OId = r.HorseOId
			JOIN Horse hm ON hm.OId = r.ParentOId
			WHERE r.ParentType = 'Mother' AND h.isDeleted = 0
			GROUP BY h.Age, hm.OId
			HAVING COUNT(*) > 1

			SELECT 
			h.Id, h.OId, h.[Name], h.Age, h.Sex, h.Country, h.Family, hf.Id FatherId, hf.OId FatherOId, hf.[Name] FatherName, hm.Id MotherId, hm.OId MotherOId, hm.[Name] MotherName, t.TwinYear, t.Twins 
			FROM @tblTwin t
			LEFT JOIN Horse h ON h.OId=t.OId
			LEFT JOIN Relationship rf ON rf.HorseOId = t.OId AND rf.ParentType = 'Father'
			LEFT JOIN Horse hf ON hf.OId = rf.ParentOId
			LEFT JOIN Relationship rm ON rm.HorseOId = t.OId AND rm.ParentType = 'Mother'
			LEFT JOIN Horse hm ON hm.OId = rm.ParentOId
		";

		public const string Get_Founder_List_Query = @"
			SELECT DISTINCT h.*
			FROM Horse h
			WHERE 
				-- Is parent
				h.OId IN (SELECT DISTINCT ParentOId FROM Relationship WHERE ParentOId IS NOT NULL AND ParentType='Mother') AND 
				-- Has no parent
				h.OId NOT IN (SELECT DISTINCT HorseOId FROM Relationship WHERE HorseOId IS NOT NULL) AND
				h.isDeleted = 0 AND
				h.Age < 1999;
		";

		public const string Remove_Unnecessary_Inbreeds = "DELETE FROM Inbreed WHERE DATEDIFF(minute, CreatedAt, CURRENT_TIMESTAMP) > 30";

		public const string Attach_Parents_Query = @"
			SELECT h.Id, hf.Id FatherId, hf.OId FatherOId, hf.[Name] FatherName, hm.Id MotherId, hm.OId MotherOId, hm.[Name] MotherName 
			FROM Horse h 
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
			LEFT JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother'
			LEFt JOIN Horse hm ON hm.OId=rm.ParentOId
			WHERE h.Id IN ({0});
		";

		public const string Update_Family_For_Tail_Female = @"
			DECLARE @HorseId int = {0}; 
			DECLARE @Family nvarchar(100) = {1};
			DECLARE @DepthLevel int = 0;

			DECLARE @tblOId as Table
			(
				HorseId integer,
    			HorseOId nvarchar(100),
    			[Name] nvarchar(200),
				Age integer,
				Sex nvarchar(15),
				Family nvarchar(15),
				Country nvarchar(15),
    			IsTraversed bit,
				Depth integer
			)
 
			IF(@HorseId Is Not NULL)
				BEGIN
        			INSERT INTO @tblOId(HorseId, HorseOId, [Name], Age, Sex, Family, Country, IsTraversed, Depth)
        			SELECT hc.Id, hc.OId, hc.[Name], hc.Age, hc.Sex, hc.Family, hc.Country, 0, 0
					FROM Horse h
					JOIN Relationship r ON r.ParentOId=h.OId
					JOIN Horse hc ON hc.OId=r.HorseOId
					WHERE h.Id=@HorseId
 		
        			While((SELECT Count(1) FROM @tblOId Where IsTraversed = 0 AND Sex = 'Female') > 0)
        				BEGIN
							SELECT top 1 @HorseId =  HorseId, @DepthLevel = Depth FROM @tblOId Where IsTraversed = 0 AND Sex = 'Female'
			
            				INSERT INTO @tblOId(HorseId, HorseOId, [Name], Age, Sex, Family, Country, IsTraversed, Depth)
        					SELECT hc.Id, hc.OId, hc.[Name], hc.Age, hc.Sex, hc.Family, hc.Country, 0, @DepthLevel+1
							FROM Horse h
							JOIN Relationship r ON r.ParentOId=h.OId
							JOIN Horse hc ON hc.OId=r.HorseOId
							WHERE h.Id=@HorseId

            				UPDATE @tblOId
            				SET IsTraversed = 1
            				WHERE HorseId = @HorseId and Depth = @DepthLevel
 
        				End

        				UPDATE Horse SET Family = @Family WHERE Id IN (SELECT HorseId FROM @tblOId)
    			End
			";

        public const string Update_MtDNA_For_Tail_Female = @"
			DECLARE @HorseId int = {0}; 
			DECLARE @MtDNA nvarchar(100) = {1};
			DECLARE @DepthLevel int = 0;

			DECLARE @tblOId as Table
			(
				HorseId integer,
    			HorseOId nvarchar(100),
    			[Name] nvarchar(200),
				Age integer,
				Sex nvarchar(15),
				Family nvarchar(15),
				Country nvarchar(15),
    			IsTraversed bit,
				Depth integer
			)
 
			IF(@HorseId Is Not NULL)
				BEGIN
        			INSERT INTO @tblOId(HorseId, HorseOId, [Name], Age, Sex, Family, Country, IsTraversed, Depth)
        			SELECT hc.Id, hc.OId, hc.[Name], hc.Age, hc.Sex, hc.Family, hc.Country, 0, 0
					FROM Horse h
					JOIN Relationship r ON r.ParentOId=h.OId
					JOIN Horse hc ON hc.OId=r.HorseOId
					WHERE h.Id=@HorseId
 		
        			While((SELECT Count(1) FROM @tblOId Where IsTraversed = 0 AND Sex = 'Female') > 0)
        				BEGIN
							SELECT top 1 @HorseId =  HorseId, @DepthLevel = Depth FROM @tblOId Where IsTraversed = 0 AND Sex = 'Female'
			
            				INSERT INTO @tblOId(HorseId, HorseOId, [Name], Age, Sex, Family, Country, IsTraversed, Depth)
        					SELECT hc.Id, hc.OId, hc.[Name], hc.Age, hc.Sex, hc.Family, hc.Country, 0, @DepthLevel+1
							FROM Horse h
							JOIN Relationship r ON r.ParentOId=h.OId
							JOIN Horse hc ON hc.OId=r.HorseOId
							WHERE h.Id=@HorseId

            				UPDATE @tblOId
            				SET IsTraversed = 1
            				WHERE HorseId = @HorseId and Depth = @DepthLevel
 
        				End

        				UPDATE Horse SET MtDNA = @MtDNA WHERE Id IN (SELECT HorseId FROM @tblOId)
    			End
			";

		public const string Horses_By_HaploTypes = @"
			SELECT * FROM Horse WHERE Age >= 1991 AND MtDNA IN ({0});
		";

		public const string Calculate_Stallion_Rating = @"
			DECLARE @LoopCounter INT = 1
				DECLARE @MaxId INT
				DECLARE @HorseOId NVARCHAR(100)

				DECLARE @CurrentRCount INT, @CurrentZCount INT
				DECLARE @HistoricalRCount INT, @HistoricalZCount INT
				DECLARE @BMSCurrentRCount INT, @BMSCurrentZCount INT
				DECLARE @BMSHistoricalRCount INT, @BMSHistoricalZCount INT
				DECLARE @SOSCurrentSCount INT, @SOSCurrentRCount INT, @SOSCurrentZCount INT
				DECLARE @SOSHistoricalSCount INT, @SOSHistoricalRCount INT, @SOSHistoricalZCount INT
				DECLARE @BMSOSCurrentSCount INT, @BMSOSCurrentRCount INT, @BMSOSCurrentZCount INT
				DECLARE @BMSOSHistoricalSCount INT, @BMSOSHistoricalRCount INT, @BMSOSHistoricalZCount INT

				DECLARE @Wnrs FLOAT, @Rnrs FLOAT, @AvgRnrs FLOAT, @PRB2 FLOAT
				DECLARE @UniqueSires FLOAT, @AllRaces FLOAT

				DECLARE @OldestOffspringYOB INT

				SELECT 
					@UniqueSires = CAST(COUNT(DISTINCT rf.ParentOId) AS FLOAT)
				FROM Position p
				LEFT JOIN Horse h ON h.OId=p.HorseOId
				LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
				WHERE p.Place=1 AND rf.ParentOId IS NOT NULL

				SELECT @AllRaces=CAST(COUNT(*) AS FLOAT) FROM Race

				-- Delete old StallionRating data
				DELETE FROM StallionRating

				SELECT @MaxId = ISNULL(MAX(Id), 0) FROM StallionRating
				DBCC CHECKIDENT (StallionRating, RESEED, @MaxId)

				INSERT INTO StallionRating(HorseOId) 
				SELECT ParentOId 
				FROM Relationship WHERE ParentType='Father' GROUP BY ParentOId 


				SELECT @MaxId=MAX(Id) FROM StallionRating

				WHILE(@LoopCounter <= @MaxId)
					BEGIN
						SELECT @HorseOId = HorseOId FROM StallionRating WHERE Id = @LoopCounter
 
						SELECT
							@CurrentRCount = COUNT(h.OId)
						FROM Horse h
						JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.CurrentBPR IS NOT NULL
						WHERE rf.ParentOId=@HorseOId

						SELECT
							@CurrentZCount = COUNT(h.OId)
						FROM Horse h
						JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZCurrentBPR IS NOT NULL
						WHERE rf.ParentOId=@HorseOId

						SELECT
							@HistoricalRCount = COUNT(h.OId)
						FROM Horse h
						JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.HistoricalBPR IS NOT NULL
						WHERE rf.ParentOId=@HorseOId

						SELECT
							@HistoricalZCount = COUNT(h.OId)
						FROM Horse h
						JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZHistoricalBPR IS NOT NULL
						WHERE rf.ParentOId=@HorseOId

						SELECT
							@BMSCurrentRCount = COUNT(h.OId)
						FROM Horse h
						-- Mother of subject horse
						JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother'
						-- Father of mother of subject horse
						JOIN Relationship rgf ON rgf.HorseOId=rm.ParentOId AND rgf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.CurrentBPR IS NOT NULL
						WHERE rgf.ParentOId=@HorseOId

						SELECT
							@BMSCurrentZCount = COUNT(h.OId)
						FROM Horse h
						-- Mother of subject horse
						JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother'
						-- Father of mother of subject horse
						JOIN Relationship rgf ON rgf.HorseOId=rm.ParentOId AND rgf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZCurrentBPR IS NOT NULL
						WHERE rgf.ParentOId=@HorseOId

						SELECT
							@BMSHistoricalRCount = COUNT(h.OId)
						FROM Horse h
						-- Mother of subject horse
						JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother'
						-- Father of mother of subject horse
						JOIN Relationship rgf ON rgf.HorseOId=rm.ParentOId AND rgf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.HistoricalBPR IS NOT NULL
						WHERE rgf.ParentOId=@HorseOId

						SELECT
							@BMSHistoricalZCount = COUNT(h.OId)
						FROM Horse h
						-- Mother of subject horse
						JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother'
						-- Father of mother of subject horse
						JOIN Relationship rgf ON rgf.HorseOId=rm.ParentOId AND rgf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZHistoricalBPR IS NOT NULL
						WHERE rgf.ParentOId=@HorseOId

						SELECT
							@SOSCurrentRCount = COUNT(h.OId)
						FROM Horse h
						-- Father of subject horse
						JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						-- Father of father of subject horse
						JOIN Relationship rgf ON rgf.HorseOId=rf.ParentOId AND rgf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.CurrentBPR IS NOT NULL
						WHERE rgf.ParentOId=@HorseOId

						SELECT
							@SOSCurrentSCount = COUNT(DISTINCT rf.ParentOId)
						FROM Horse h
						-- Father of subject horse
						JOIN Relationship rf ON rf.HorseOId = h.OId AND rf.ParentType = 'Father'
						-- Father of father of subject horse
						JOIN Relationship rgf ON rgf.HorseOId = rf.ParentOId AND rgf.ParentType = 'Father'
						JOIN Coefficient c ON c.HorseOId=h.Oid AND c.CurrentBPR IS NOT NULL
						WHERE rgf.ParentOId=@HorseOId

						SELECT
							@SOSCurrentZCount = COUNT(h.OId)
						FROM Horse h
						-- Father of subject horse
						JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						-- Father of father of subject horse
						JOIN Relationship rgf ON rgf.HorseOId=rf.ParentOId AND rgf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZCurrentBPR IS NOT NULL
						WHERE rgf.ParentOId=@HorseOId

						SELECT
							@SOSHistoricalRCount = COUNT(h.OId)
						FROM Horse h
						-- Father of subject horse
						JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						-- Father of father of subject horse
						JOIN Relationship rgf ON rgf.HorseOId=rf.ParentOId AND rgf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.HistoricalBPR IS NOT NULL
						WHERE rgf.ParentOId=@HorseOId

						SELECT
							@SOSHistoricalSCount = COUNT(DISTINCT rf.ParentOId)
						FROM Horse h
						-- Father of subject horse
						JOIN Relationship rf ON rf.HorseOId = h.OId AND rf.ParentType = 'Father'
						-- Father of father of subject horse
						JOIN Relationship rgf ON rgf.HorseOId = rf.ParentOId AND rgf.ParentType = 'Father'
						JOIN Coefficient c ON c.HorseOId=h.Oid AND c.HistoricalBPR IS NOT NULL
						WHERE rgf.ParentOId=@HorseOId

						SELECT
							@SOSHistoricalZCount = COUNT(h.OId)
						FROM Horse h
						-- Father of subject horse
						JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						-- Father of father of subject horse
						JOIN Relationship rgf ON rgf.HorseOId=rf.ParentOId AND rgf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZHistoricalBPR IS NOT NULL
						WHERE rgf.ParentOId=@HorseOId

						SELECT
							@BMSOSCurrentRCount = COUNT(h.OId)
						FROM Horse h
						-- Father of subject horse
						JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						-- Mother of father of subject horse
						JOIN Relationship rgm ON rgm.HorseOId=rf.ParentOId AND rgm.ParentType='Mother'
						-- Father of mother of father of subject horse
						JOIN Relationship rggf ON rggf.HorseOId=rgm.ParentOId AND rggf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.CurrentBPR IS NOT NULL
						WHERE rggf.ParentOId=@HorseOId

						SELECT
							@BMSOSCurrentSCount = COUNT(DISTINCT hf.OId)
						FROM Horse h
						-- Father of subject horse
						JOIN Relationship rf ON rf.HorseOId = h.OId AND rf.ParentType = 'Father'
						JOIN Horse hf ON hf.OId=rf.ParentOId
						-- Mother of father of subject horse
						JOIN Relationship rgm ON rgm.HorseOId=hf.OId AND rgm.ParentType='Mother'
						JOIN Horse hgm ON hgm.OId=rgm.ParentOId
						-- Father of mother of father of subject horse
						JOIN Relationship rggf ON rggf.HorseOId = hgm.OId AND rggf.ParentType = 'Father'
						JOIN Horse hggf ON hggf.OId=rggf.ParentOId
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.CurrentBPR IS NOT NULL
						WHERE hggf.OId=@HorseOId

						SELECT
							@BMSOSCurrentZCount = COUNT(h.OId)
						FROM Horse h
						-- Father of subject horse
						JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						-- Mother of father of subject horse
						JOIN Relationship rgm ON rgm.HorseOId=rf.ParentOId AND rgm.ParentType='Mother'
						-- Father of mother of father of subject horse
						JOIN Relationship rggf ON rggf.HorseOId=rgm.ParentOId AND rggf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZCurrentBPR IS NOT NULL
						WHERE rggf.ParentOId=@HorseOId

						SELECT
							@BMSOSHistoricalRCount = COUNT(h.OId)
						FROM Horse h
						-- Father of subject horse
						JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						-- Mother of father of subject horse
						JOIN Relationship rgm ON rgm.HorseOId=rf.ParentOId AND rgm.ParentType='Mother'
						-- Father of mother of father of subject horse
						JOIN Relationship rggf ON rggf.HorseOId=rgm.ParentOId AND rggf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.HistoricalBPR IS NOT NULL
						WHERE rggf.ParentOId=@HorseOId

						SELECT
							@BMSOSHistoricalSCount = COUNT(DISTINCT hf.OId)
						FROM Horse h
						-- Father of subject horse
						JOIN Relationship rf ON rf.HorseOId = h.OId AND rf.ParentType = 'Father'
						JOIN Horse hf ON hf.OId=rf.ParentOId
						-- Mother of father of subject horse
						JOIN Relationship rgm ON rgm.HorseOId=hf.OId AND rgm.ParentType='Mother'
						JOIN Horse hgm ON hgm.OId=rgm.ParentOId
						-- Father of mother of father of subject horse
						JOIN Relationship rggf ON rggf.HorseOId = hgm.OId AND rggf.ParentType = 'Father'
						JOIN Horse hggf ON hggf.OId=rggf.ParentOId
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.HistoricalBPR IS NOT NULL
						WHERE hggf.OId=@HorseOId

						SELECT
							@BMSOSHistoricalZCount = COUNT(h.OId)
						FROM Horse h
						-- Father of subject horse
						JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						-- Mother of father of subject horse
						JOIN Relationship rgm ON rgm.HorseOId=rf.ParentOId AND rgm.ParentType='Mother'
						-- Father of mother of father of subject horse
						JOIN Relationship rggf ON rggf.HorseOId=rgm.ParentOId AND rggf.ParentType='Father'
						JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZHistoricalBPR IS NOT NULL
						WHERE rggf.ParentOId=@HorseOId

						SELECT
							@OldestOffspringYOB = MIN(h.Age)
						FROM Horse h
						JOIN Relationship r ON r.HorseOId=h.OId
						WHERE r.ParentOId=@HorseOId

						-- Update StallionRating
						UPDATE StallionRating 
						SET 
						-- Current SR 
						CurrentRCount=@CurrentRCount,
						CurrentZCount=@CurrentZCount,
						CurrentStallionRating=(
							CASE WHEN @CurrentRCount >= 3 AND @CurrentZCount >= 3 THEN
								(SELECT
									SUM(c.ZCurrentBPR)
								FROM Horse h
								JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
								JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZCurrentBPR IS NOT NULL
								WHERE rf.ParentOId=@HorseOId)
							ELSE 
								(SELECT NULL)
							END
						),
 						-- Historical SR 
						HistoricalRCount=@HistoricalRCount,
						HistoricalZCount=@HistoricalZCount,
						HistoricalStallionRating=(
							CASE WHEN @HistoricalRCount >= 3 AND @HistoricalZCount >= 3 THEN
								(SELECT
									SUM(c.ZHistoricalBPR)
								FROM Horse h
								JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
								JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZHistoricalBPR IS NOT NULL
								WHERE rf.ParentOId=@HorseOId)
							ELSE
								(SELECT NULL)
							END
						),
						-- Current BMS SR
						BMSCurrentRCount=@BMSCurrentRCount,
						BMSCurrentZCount=@BMSCurrentZCount,
						BMSCurrentStallionRating=(
							CASE WHEN @BMSCurrentRCount >= 3 AND @BMSCurrentZCount >= 3 THEN
								(
									SELECT
										SUM(c.ZCurrentBPR)
									FROM Horse h
									-- Mother of subject horse
									JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother'
									-- Father of mother of subject horse
									JOIN Relationship rgf ON rgf.HorseOId=rm.ParentOId AND rgf.ParentType='Father'
									JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZCurrentBPR IS NOT NULL
									WHERE rgf.ParentOId=@HorseOId
								)
							ELSE
								(SELECT NULL)
							END
						),
						-- Historical BMS SR
						BMSHistoricalRCount=@BMSHistoricalRCount,
						BMSHistoricalZCount=@BMSHistoricalZCount,
						BMSHistoricalStallionRating=(
							CASE WHEN @BMSHistoricalRCount >= 3 AND @BMSHistoricalZCount >= 3 THEN
								(
									SELECT
										SUM(c.ZHistoricalBPR)
									FROM Horse h
									-- Mother of subject horse
									JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother'
									-- Father of mother of subject horse
									JOIN Relationship rgf ON rgf.HorseOId=rm.ParentOId AND rgf.ParentType='Father'
									JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZHistoricalBPR IS NOT NULL
									WHERE rgf.ParentOId=@HorseOId
								)
							ELSE
								(SELECT NULL)
							END
						),
						-- Current SOS SR
						SOSCurrentRCount=@SOSCurrentRCount,
						SOSCurrentSCount=@SOSCurrentSCount,
						SOSCurrentZCount=@SOSCurrentZCount,
						SOSCurrentStallionRating=(
							CASE WHEN @SOSCurrentRCount >= 3 AND @SOSCurrentZCount >= 3 THEN
								(
									SELECT
										SUM(c.ZCurrentBPR)
									FROM Horse h
									-- Father of subject horse
									JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
									-- Father of father of subject horse
									JOIN Relationship rgf ON rgf.HorseOId=rf.ParentOId AND rgf.ParentType='Father'
									JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZCurrentBPR IS NOT NULL
									WHERE rgf.ParentOId=@HorseOId
								)
							ELSE
								(SELECT NULL)
							END
						),
						-- Historical SOS SR
						SOSHistoricalRCount=@SOSHistoricalRCount,
						SOSHistoricalSCount=@SOSHistoricalSCount,
						SOSHistoricalZCount=@SOSHistoricalZCount,
						SOSHistoricalStallionRating=(
							CASE WHEN @SOSHistoricalRCount >= 3 AND @SOSHistoricalZCount >= 3 THEN
								(
									SELECT
										SUM(c.ZHistoricalBPR)
									FROM Horse h
									-- Father of subject horse
									JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
									-- Father of father of subject horse
									JOIN Relationship rgf ON rgf.HorseOId=rf.ParentOId AND rgf.ParentType='Father'
									JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZHistoricalBPR IS NOT NULL
									WHERE rgf.ParentOId=@HorseOId
								)
							ELSE
								(SELECT NULL)
							END
						),
						-- Current BMSOS SR
						BMSOSCurrentRCount=@BMSOSCurrentRCount,
						BMSOSCurrentSCount=@BMSOSCurrentSCount,
						BMSOSCurrentZCount=@BMSOSCurrentZCount,
						BMSOSCurrentStallionRating=(
							CASE WHEN @BMSOSCurrentRCount >= 3 AND @BMSOSCurrentZCount >= 3 THEN
								(
									SELECT
										SUM(c.ZCurrentBPR)
									FROM Horse h
									-- Father of subject horse
									JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
									-- Mother of father of subject horse
									JOIN Relationship rgm ON rgm.HorseOId=rf.ParentOId AND rgm.ParentType='Mother'
									-- Father of mother of father of subject horse
									JOIN Relationship rggf ON rggf.HorseOId=rgm.ParentOId AND rggf.ParentType='Father'
									JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZCurrentBPR IS NOT NULL
									WHERE rggf.ParentOId=@HorseOId
								)
							ELSE
								(SELECT NULL)
							END
						),
						-- Historical BMSOS SR
						BMSOSHistoricalRCount=@BMSOSHistoricalRCount,
						BMSOSHistoricalSCount=@BMSOSHistoricalSCount,
						BMSOSHistoricalZCount=@BMSOSHistoricalZCount,
						BMSOSHistoricalStallionRating=(
							CASE WHEN @BMSOSHistoricalRCount >= 3 AND @BMSOSHistoricalZCount >= 3 THEN
								(
									SELECT
										SUM(c.ZHistoricalBPR)
									FROM Horse h
									-- Father of subject horse
									JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
									-- Mother of father of subject horse
									JOIN Relationship rgm ON rgm.HorseOId=rf.ParentOId AND rgm.ParentType='Mother'
									-- Father of mother of father of subject horse
									JOIN Relationship rggf ON rggf.HorseOId=rgm.ParentOId AND rggf.ParentType='Father'
									JOIN Coefficient c ON c.HorseOId=h.OId AND c.ZHistoricalBPR IS NOT NULL
									WHERE rggf.ParentOId=@HorseOId
								)
							ELSE
								(SELECT NULL)
							END
						),
						-- CropAge
						CropAge=YEAR(GETDATE()) - @OldestOffspringYOB--,
						--IV=@Wnrs/@Rnrs/(1/@AvgRnrs)
						WHERE HorseOId=@HorseOId

		

						-- IV, A/E, PRB^2
						-- Offspring winners count for stallion
						SELECT 
							@Wnrs = CAST(COUNT(h.Id) AS FLOAT)
						FROM Position p
						LEFT JOIN Horse h ON h.OId=p.HorseOId
						LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						WHERE p.Place=1 AND rf.ParentOId=@HorseOId
		
						-- Offspring runners count for stallion
						SELECT 
							@Rnrs = CAST(COUNT(h.Id) AS FLOAT)
						FROM Position p
						LEFT JOIN Horse h ON h.OId=p.HorseOId
						LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						WHERE rf.ParentOId=@HorseOId

						-- Average field size of race for stallion
						SELECT
							@AvgRnrs = CAST(SUM(Rnrs) AS FLOAT) / CAST(COUNT(Rnrs) AS FLOAT)
						FROM 
						(
							SELECT
								(SELECT COUNT(*) FROM Position WHERE RaceId=r.Id) Rnrs
							FROM Position p
							LEFT JOIN Race r ON r.Id=p.RaceId
							LEFT JOIN Relationship rf ON rf.HorseOId=p.HorseOId AND rf.ParentType='Father'
							WHERE rf.ParentOId=@HorseOId
						) a

						-- Average PRB2 for stallion
						SELECT 
							@PRB2 = SUM(PRB2) / COUNT(PRB2)
						FROM
						(
						SELECT 
							(POWER(CAST((SELECT COUNT(*) FROM Position WHERE RaceId=r.Id) - p.Place AS FLOAT) / CAST((SELECT COUNT(*) FROM Position WHERE RaceId=r.Id) - 1 AS FLOAT), 2)) PRB2
						FROM Position p
						LEFT JOIN Race r ON r.Id=p.RaceId
						LEFT JOIN Horse h ON h.OId=p.HorseOId
						LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
						WHERE rf.ParentOId=@HorseOId
						) a


						IF (@Rnrs > 0 AND @AvgRnrs > 0) UPDATE StallionRating SET IV=@Wnrs/@Rnrs/(1/@AvgRnrs) WHERE HorseOId=@HorseOId
						IF (@Rnrs > 0) UPDATE StallionRating SET AE=(@Wnrs/@Rnrs)/(@UniqueSires/@AllRaces) WHERE HorseOId=@HorseOId
						UPDATE StallionRating SET PRB2=@PRB2 WHERE HorseOId=@HorseOId

 						SET @LoopCounter  = @LoopCounter  + 1        
					END

				DECLARE @MaxCurrentStallionRating float, @MinCurrentStallionRating float
				DECLARE @MaxHistoricalStallionRating float, @MinHistoricalStallionRating float
				DECLARE @MaxBMSCurrentStallionRating float, @MinBMSCurrentStallionRating float
				DECLARE @MaxBMSHistoricalStallionRating float, @MinBMSHistoricalStallionRating float
				DECLARE @MaxSOSCurrentStallionRating float, @MinSOSCurrentStallionRating float
				DECLARE @MaxSOSHistoricalStallionRating float, @MinSOSHistoricalStallionRating float
				DECLARE @MaxBMSOSCurrentStallionRating float, @MinBMSOSCurrentStallionRating float
				DECLARE @MaxBMSOSHistoricalStallionRating float, @MinBMSOSHistoricalStallionRating float

				SELECT 
					@MaxCurrentStallionRating = MAX(CurrentStallionRating), 
					@MinCurrentStallionRating = MIN(CurrentStallionRating), 
					@MaxHistoricalStallionRating = MAX(HistoricalStallionRating), 
					@MinHistoricalStallionRating = MIN(HistoricalStallionRating), 

					@MaxBMSCurrentStallionRating = MAX(BMSCurrentStallionRating), 
					@MinBMSCurrentStallionRating = MIN(BMSCurrentStallionRating), 
					@MaxBMSHistoricalStallionRating = MAX(BMSHistoricalStallionRating), 
					@MinBMSHistoricalStallionRating = MIN(BMSHistoricalStallionRating), 

					@MaxSOSCurrentStallionRating = MAX(SOSCurrentStallionRating), 
					@MinSOSCurrentStallionRating = MIN(SOSCurrentStallionRating), 
					@MaxSOSHistoricalStallionRating = MAX(SOSHistoricalStallionRating), 
					@MinSOSHistoricalStallionRating = MIN(SOSHistoricalStallionRating), 

					@MaxBMSOSCurrentStallionRating = MAX(BMSOSCurrentStallionRating), 
					@MinBMSOSCurrentStallionRating = MIN(BMSOSCurrentStallionRating), 
					@MaxBMSOSHistoricalStallionRating = MAX(BMSOSHistoricalStallionRating), 
					@MinBMSOSHistoricalStallionRating = MIN(BMSOSHistoricalStallionRating)
				FROM StallionRating

				UPDATE 
					StallionRating 
				SET 
					CurrentStallionRating = ((CurrentStallionRating-@MinCurrentStallionRating)/(@MaxCurrentStallionRating-@MinCurrentStallionRating))*(100-1)+1,
					HistoricalStallionRating = ((HistoricalStallionRating-@MinHistoricalStallionRating)/(@MaxHistoricalStallionRating-@MinHistoricalStallionRating))*(100-1)+1,
					BMSCurrentStallionRating = ((BMSCurrentStallionRating-@MinBMSCurrentStallionRating)/(@MaxBMSCurrentStallionRating-@MinBMSCurrentStallionRating))*(100-1)+1,
					BMSHistoricalStallionRating = ((BMSHistoricalStallionRating-@MinBMSHistoricalStallionRating)/(@MaxBMSHistoricalStallionRating-@MinBMSHistoricalStallionRating))*(100-1)+1,
					SOSCurrentStallionRating = ((SOSCurrentStallionRating-@MinSOSCurrentStallionRating)/(@MaxSOSCurrentStallionRating-@MinSOSCurrentStallionRating))*(100-1)+1,
					SOSHistoricalStallionRating = ((SOSHistoricalStallionRating-@MinSOSHistoricalStallionRating)/(@MaxSOSHistoricalStallionRating-@MinSOSHistoricalStallionRating))*(100-1)+1,
					BMSOSCurrentStallionRating = ((BMSOSCurrentStallionRating-@MinBMSOSCurrentStallionRating)/(@MaxBMSOSCurrentStallionRating-@MinBMSOSCurrentStallionRating))*(100-1)+1,
					BMSOSHistoricalStallionRating = ((BMSOSHistoricalStallionRating-@MinBMSOSHistoricalStallionRating)/(@MaxBMSOSHistoricalStallionRating-@MinBMSOSHistoricalStallionRating))*(100-1)+1

				UPDATE StallionRating SET CurrentStallionRating=0 WHERE CurrentStallionRating IS NULL
				UPDATE StallionRating SET HistoricalStallionRating=0 WHERE HistoricalStallionRating IS NULL
				UPDATE StallionRating SET BMSCurrentStallionRating=0 WHERE BMSCurrentStallionRating IS NULL
				UPDATE StallionRating SET BMSHistoricalStallionRating=0 WHERE BMSHistoricalStallionRating IS NULL
				UPDATE StallionRating SET SOSCurrentStallionRating=0 WHERE SOSCurrentStallionRating IS NULL
				UPDATE StallionRating SET SOSHistoricalStallionRating=0 WHERE SOSHistoricalStallionRating IS NULL
				UPDATE StallionRating SET BMSOSCurrentStallionRating=0 WHERE BMSOSCurrentStallionRating IS NULL
				UPDATE StallionRating SET BMSOSHistoricalStallionRating=0 WHERE BMSOSHistoricalStallionRating IS NULL
			";

		public const string Get_Stallion_Ratings_Paged_Query = @"
			SELECT 
				h.[Name] HorseName, 
				s.*
			FROM StallionRating s 
			JOIN Horse h ON h.OId=s.HorseOId
			WHERE h.[Name] LIKE '%{0}%' AND h.isDeleted = 0 AND ({1})
			ORDER BY {2}
			OFFSET (@PageNumber-1) * @PageSize ROWS
			FETCH NEXT @PageSize ROWS ONLY;
			
			SELECT COUNT(*) FROM StallionRating s JOIN Horse h ON h.OId=s.HorseOId WHERE h.[Name] LIKE '%{0}%' AND h.isDeleted = 0 AND ({1})
		";

		public const string Calculate_COID_Query = @"
			DECLARE @LoopCounter INT = 1
			DECLARE @MaxId INT
			DECLARE @HorseOId NVARCHAR(100), @HorseOId1 NVARCHAR(100), @HorseOId2 NVARCHAR(100), @HorseOId3 NVARCHAR(100), @HorseOId4 NVARCHAR(100), @HorseOId5 NVARCHAR(100), @HorseOId6 NVARCHAR(100)
			DECLARE @COI FLOAT = 0, @COI1 FLOAT = 0, @COI2 FLOAT = 0, @COI3 FLOAT = 0, @COI4 FLOAT = 0, @COI5 FLOAT = 0, @COI6 FLOAT = 0

			SELECT @MaxId=MAX(Id) FROM Coefficient

			WHILE(@LoopCounter <= @MaxId)
				BEGIN
					SELECT @HorseOId = HorseOId, @COI = COI FROM Coefficient WHERE Id = @LoopCounter

					-- COID1
					SELECT @HorseOId1 = ParentOId FROM Relationship WHERE HorseOId=@HorseOId AND ParentType='Father'
					IF (@HorseOId1 IS NOT NULL) SELECT @COI1 = COI FROM Coefficient WHERE HorseOId=@HorseOId1

					-- COID2
					SELECT @HorseOId2 = ParentOId FROM Relationship WHERE HorseOId=@HorseOId AND ParentType='Mother'
					IF (@HorseOId2 IS NOT NULL) SELECT @COI2 = COI FROM Coefficient WHERE HorseOId=@HorseOId2

					-- COID3
					SELECT @HorseOId3 = rgf.ParentOId FROM Relationship rf JOIN Relationship rgf ON rgf.HorseOId=rf.ParentOId AND rgf.ParentType='Father' WHERE rf.HorseOId=@HorseOId AND rf.ParentType='Father'		
					IF (@HorseOId3 IS NOT NULL) SELECT @COI3 = COI FROM Coefficient WHERE HorseOId=@HorseOId3

					-- COID4
					SELECT @HorseOId4 = rgm.ParentOId FROM Relationship rf JOIN Relationship rgm ON rgm.HorseOId=rf.ParentOId AND rgm.ParentType='Mother' WHERE rf.HorseOId=@HorseOId AND rf.ParentType='Father'		
					IF (@HorseOId4 IS NOT NULL) SELECT @COI4 = COI FROM Coefficient WHERE HorseOId=@HorseOId4

					-- COID5
					SELECT @HorseOId5 = rgf.ParentOId FROM Relationship rm JOIN Relationship rgf ON rgf.HorseOId=rm.ParentOId AND rgf.ParentType='Father' WHERE rm.HorseOId=@HorseOId AND rm.ParentType='Mother'		
					IF (@HorseOId5 IS NOT NULL) SELECT @COI5 = COI FROM Coefficient WHERE HorseOId=@HorseOId5

					-- COID6
					SELECT @HorseOId6 = rgm.ParentOId FROM Relationship rm JOIN Relationship rgm ON rgm.HorseOId=rm.ParentOId AND rgm.ParentType='Mother' WHERE rm.HorseOId=@HorseOId AND rm.ParentType='Mother'		
					IF (@HorseOId6 IS NOT NULL) SELECT @COI6 = COI FROM Coefficient WHERE HorseOId=@HorseOId6


					UPDATE Coefficient SET COID1 = @COI - @COI1, COID2 = @COI - @COI2, COID3 = @COI - @COI3, COID4 = @COI - @COI4, COID5 = @COI - @COI5, COID6 = @COI - @COI6 WHERE Id = @LoopCounter

 					SET @LoopCounter  = @LoopCounter  + 1  
				END
			";

		public const string Calculate_COID_For_Horse_Query = @"
			DECLARE @HorseOId NVARCHAR(100) = '{0}'
			DECLARE @COI FLOAT = 0
			DECLARE @HorseOId1 NVARCHAR(100), @HorseOId2 NVARCHAR(100), @HorseOId3 NVARCHAR(100), @HorseOId4 NVARCHAR(100), @HorseOId5 NVARCHAR(100), @HorseOId6 NVARCHAR(100)
			DECLARE @COI1 FLOAT = 0, @COI2 FLOAT = 0, @COI3 FLOAT = 0, @COI4 FLOAT = 0, @COI5 FLOAT = 0, @COI6 FLOAT = 0
			
			SELECT @COI = COI FROM Coefficient WHERE HorseOId=@HorseOId

			-- COID1
			SELECT @HorseOId1 = ParentOId FROM Relationship WHERE HorseOId=@HorseOId AND ParentType='Father'
			IF (@HorseOId1 IS NOT NULL) SELECT @COI1 = COI FROM Coefficient WHERE HorseOId=@HorseOId1

			-- COID2
			SELECT @HorseOId2 = ParentOId FROM Relationship WHERE HorseOId=@HorseOId AND ParentType='Mother'
			IF (@HorseOId2 IS NOT NULL) SELECT @COI2 = COI FROM Coefficient WHERE HorseOId=@HorseOId2

			-- COID3
			SELECT @HorseOId3 = rgf.ParentOId FROM Relationship rf JOIN Relationship rgf ON rgf.HorseOId=rf.ParentOId AND rgf.ParentType='Father' WHERE rf.HorseOId=@HorseOId AND rf.ParentType='Father'		
			IF (@HorseOId3 IS NOT NULL) SELECT @COI3 = COI FROM Coefficient WHERE HorseOId=@HorseOId3

			-- COID4
			SELECT @HorseOId4 = rgm.ParentOId FROM Relationship rf JOIN Relationship rgm ON rgm.HorseOId=rf.ParentOId AND rgm.ParentType='Mother' WHERE rf.HorseOId=@HorseOId AND rf.ParentType='Father'		
			IF (@HorseOId4 IS NOT NULL) SELECT @COI4 = COI FROM Coefficient WHERE HorseOId=@HorseOId4

			-- COID5
			SELECT @HorseOId5 = rgf.ParentOId FROM Relationship rm JOIN Relationship rgf ON rgf.HorseOId=rm.ParentOId AND rgf.ParentType='Father' WHERE rm.HorseOId=@HorseOId AND rm.ParentType='Mother'		
			IF (@HorseOId5 IS NOT NULL) SELECT @COI5 = COI FROM Coefficient WHERE HorseOId=@HorseOId5

			-- COID6
			SELECT @HorseOId6 = rgm.ParentOId FROM Relationship rm JOIN Relationship rgm ON rgm.HorseOId=rm.ParentOId AND rgm.ParentType='Mother' WHERE rm.HorseOId=@HorseOId AND rm.ParentType='Mother'		
			IF (@HorseOId6 IS NOT NULL) SELECT @COI6 = COI FROM Coefficient WHERE HorseOId=@HorseOId6


			UPDATE Coefficient SET COID1 = @COI - @COI1, COID2 = @COI - @COI2, COID3 = @COI - @COI3, COID4 = @COI - @COI4, COID5 = @COI - @COI5, COID6 = @COI - @COI6 WHERE HorseOId = @HorseOId
		";

		public const string Horse_Races_Query = @"
			SELECT 
				p.HorseOId,
				p.Place,
				r.Id RaceId, 
				r.[Name],
				r.[Date],
				r.Country,
				r.Distance,
				r.Surface,
				r.[Type],
				r.[Status],
				(SELECT SUM(CurrentBPR)/COUNT(*) FROM Coefficient WHERE HorseOId IN (SELECT HorseOId FROM Position WHERE RaceId=r.Id)) BPR			
			FROM Position p 
			JOIN Race r ON r.Id=p.RaceId
			WHERE p.HorseOId='{0}'
			ORDER BY {1}
		";

        public const string Horses_Races_Query = @"
			SELECT 
				p.HorseOId,
				p.Place,
				r.Id RaceId, 
				r.[Name],
				r.[Date],
				r.Country,
				r.Distance,
				r.Surface,
				r.[Type],
				r.[Status],
				(SELECT SUM(CurrentBPR)/COUNT(*) FROM Coefficient WHERE HorseOId IN (SELECT HorseOId FROM Position WHERE RaceId=r.Id)) BPR			
			FROM Position p 
			JOIN Race r ON r.Id=p.RaceId
			WHERE p.HorseOId IN ({0})
		";

		public const string Horse_PedigreeComp_Query = @"
			DECLARE @Id int = {0};  
			DECLARE @GenerationLevel int = {1};
			DECLARE @YOB int = {2};
			DECLARE @DepthLevel int = 0;

			DECLARE @tblOId AS Table
			(
				Id integer IDENTITY(1,1) NOT NULL,
				HorseId integer,
    			HorseOId nvarchar(max),
    			[Name] nvarchar(max),
				Age integer,
				Sex nvarchar(15),
				Family nvarchar(15),
				Country nvarchar(15),
				MtDNA integer,
				HaploGroupId integer,
				FatherId integer,
    			FatherOId nvarchar(max),
				MotherId integer,
    			MotherOId nvarchar(max),
    			IsTraversed bit,
				Depth integer,
				SD char
			)
 
			IF(@Id IS NOT NULL)
    			BEGIN
        			INSERT INTO @tblOId(HorseId, HorseOId, Name, Age, Sex, Family, Country, MtDNA, HaploGroupId, FatherId, FatherOId, MotherId, MotherOId, IsTraversed, Depth, SD)
        			SELECT h.Id, h.OId, h.Name, h.Age, h.Sex, h.Family, h.Country, h.MtDNA, t.GroupId, hf.Id FatherId, rf.ParentOId FatherOId, hm.Id MotherId, rm.ParentOId MotherOId, 0, @DepthLevel, NULL 
					FROM Horse h
					LEFT JOIN Relationship rf ON h.OId = rf.HorseOId AND rf.ParentType='Father'
					LEFT JOIN Horse hf ON hf.OId = rf.ParentOId
					LEFT JOIN Relationship rm ON h.OId = rm.HorseOId AND rm.ParentType='Mother'
					LEFT JOIN Horse hm ON hm.OId = rm.ParentOId
					LEFT JOIN HaploType t ON t.Id=h.MtDNA
					WHERE h.Id = @Id AND h.isDeleted = 0
 		
        			WHILE((SELECT Count(1) FROM @tblOId WHERE IsTraversed = 0) > 0)
        				BEGIN
							DECLARE @tblId integer;
							DECLARE @HorseOId nvarchar(max) ;
							DECLARE @FatherOId nvarchar(max) ;
							DECLARE @MotherOId nvarchar(max) ;
							DECLARE @SD nvarchar(max) ;

							IF ((SELECT Count(1) FROM @tblOId WHERE IsTraversed = 0 AND Depth = @DepthLevel) > 0)
		    						SELECT TOP 1 @tblId = Id, @HorseOId = HorseOId, @FatherOId =  FatherOId, @MotherOId =  MotherOId, @DepthLevel = Depth, @SD = SD FROM @tblOId WHERE IsTraversed = 0 AND Depth = @DepthLevel ORDER BY Id;
							ELSE
								BEGIN
									SET @DepthLevel = @DepthLevel + 1
									SELECT TOP 1 @tblId = Id, @HorseOId = HorseOId, @FatherOId =  FatherOId, @MotherOId =  MotherOId, @DepthLevel = Depth, @SD = SD FROM @tblOId WHERE IsTraversed = 0 AND Depth = @DepthLevel ORDER BY Id;
								END

							IF @DepthLevel >= @GenerationLevel
								BREAK;

							IF (@FatherOId IS NOT NULL)
								BEGIN
									IF (@DepthLevel = 0) SET @SD = 'S';
            						INSERT INTO @tblOId(HorseId, HorseOId,Name, Age,Sex, Family, Country, MtDNA, HaploGroupId, FatherId, FatherOId, MotherId, MotherOId, IsTraversed, Depth, SD)
            						SELECT h.Id, h.OId, h.Name,h.Age, h.Sex, h.Family, h.Country, h.MtDNA, t.GroupId, hf.Id, rf.ParentOId, hm.Id, rm.ParentOId, 0, @DepthLevel +  1, @SD
            						FROM Horse h
									LEFT JOIN Relationship rf ON rf.HorseOId = @FatherOId AND rf.ParentType='Father'
									LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
									LEFT JOIN Relationship rm ON rm.HorseOId = @FatherOId AND rm.ParentType='Mother'
									LEFT JOIN Horse hm ON hm.OId=rm.ParentOId
									LEFT JOIN HaploType t ON t.Id=h.MtDNA									
            						WHERE h.OId = @FatherOId AND h.isDeleted = 0 AND h.Age >= @YOB AND h.Id NOT IN (SELECT HorseId FROM @tblOId)
								END 
			
							IF (@MotherOId IS NOT NULL)
								BEGIN
            						IF (@DepthLevel = 0) SET @SD = 'D';
            						INSERT INTO @tblOId(HorseId, HorseOId,Name, Age,Sex, Family, Country, MtDNA, HaploGroupId, FatherId, FatherOId, MotherId, MotherOId, IsTraversed, Depth, SD)
            						SELECT h.Id, h.OId, h.Name,h.Age, h.Sex, h.Family, h.Country, h.MtDNA, t.GroupId, hf.Id, rf.ParentOId, hm.Id, rm.ParentOId, 0, @DepthLevel +  1, @SD
            						FROM Horse h
									LEFT JOIN Relationship rf ON rf.HorseOId = @MotherOId AND rf.ParentType='Father'
									LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
									LEFT JOIN Relationship rm ON rm.HorseOId = @MotherOId AND rm.ParentType='Mother'
									LEFT JOIN Horse hm ON hm.OId=rm.ParentOId
									LEFT JOIN HaploType t ON t.Id=h.MtDNA		
            						WHERE h.OId = @MotherOId AND h.isDeleted = 0 AND h.Age >= @YOB AND h.Id NOT IN (SELECT HorseId FROM @tblOId)
								END
			
							UPDATE @tblOId
            				SET IsTraversed = 1
            				WHERE Id = @tblId
        				END

        				SELECT Id As [No], HorseId AS Id, HorseOId AS OId, Name, Age, Sex, Family, Country, MtDNA, HaploGroupId, FatherId, FatherOId, MotherId, MotherOId, Depth, SD
        				FROM @tblOId 
						ORDER By No
    			END
			";

		public const string Horse_Pedigree_Query = @"SELECT * FROM GetPedigree(@HorseId, @Gen, @YOB)";

		public const string Horse_Hypothetical_Pedigree_Query = @"SELECT * FROM GetHypotheticalPedigree(@SireId, @DamId, @Gen, @YOB)";

        public const string Horse_Full_Pedigree_Query = @"SELECT * FROM GetFullPedigree(@HorseId, @Gen, @YOB)";

        public const string Horse_Hypothetical_Full_Pedigree_Query = @"SELECT * FROM GetHypotheticalFullPedigree(@SireId, @DamId, @Gen, @YOB)";

        public const string Get_Ancestry_List_Query = @"
				SELECT 
					h.Id, h.[Name], h.Age, h.Sex, h.Country, CONCAT(g.Title, '-', t.[Name]) MtDNATitle, g.Color MtDNAColor, a.AncestorOId, a.AvgMC
				FROM Ancestry a
				JOIN Horse h ON h.OId=a.AncestorOId
				LEFT JOIN HaploType t ON t.Id=h.MtDNA
				LEFT JOIN HaploGroup g ON g.Id=t.GroupId;
			";

		public const string Get_Unique_Ancestors = @"
			SELECT * FROM GetPedigree(@HorseId, @Gen, @YOB);
		";

		public const string Get_Grandparents_Paged_Query = @"
			SELECT 
				h.Id, 
				h.OId, 
				h.[Name], 
				h.Sex, 
				h.Age,
				h.Family, 
				CONCAT(hg.Title, '-', ht.Name) MtDNATitle,
				h.Country,
				hf.OId AS FatherOId,
				hf.[Name] AS FatherName,
				hm.OId AS MotherOId,
				hm.[Name] AS MotherName
			FROM Horse h
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
			LEFT JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother'
			LEFT JOIN Horse hm ON hm.OId=rm.ParentOId
			LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
			LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
			WHERE h.isDeleted = 0 AND ((h.Age - hf.Age) >= 25 OR (h.Age - hm.Age) >= 25)
			ORDER BY {0}
			OFFSET (@PageNumber - 1) * @PageSize ROWS
			FETCH NEXT @PageSize ROWS ONLY;

			SELECT 
				COUNT(h.Id) 
			FROM Horse h 
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father' 
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
			LEFT JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother' 
			LEFT JOIN Horse hm ON hm.OId=rm.ParentOId
			WHERE h.isDeleted = 0 AND ((h.Age - hf.Age) >= 25 OR (h.Age - hm.Age) >= 25);
		";

        public const string Get_SW_Offspring_By_Horse = @"
			SELECT DISTINCT
				h.*,
				CONCAT(hg.Title, '-', ht.Name) MtDNATitle,
				hg.Color MtDNAColor,
				dbo.GetBestRaceClass(h.OId) BestRaceClass,
				hf.Id FatherId,
				hf.OId FatherOId,
				hf.[Name] FatherName,
				hm.Id MotherId,
				hm.OId MotherOId,
				hm.[Name] MotherName
			FROM Horse h 
			JOIN RelatiONship r ON h.OId = r.HorseOId
			LEFT JOIN RelatiONship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father' 
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId 
			LEFT JOIN RelatiONship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother' 
			LEFT JOIN Horse hm ON hm.OId=rm.ParentOId 
			LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
			LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
			LEFT JOIN Position p ON p.HorseOId=h.OId
			LEFT JOIN Pedig ON Pedig.HorseOId=h.OId
			WHERE 
			r.ParentOId = (SELECT OId FROM Horse WHERE Id = @MaleId) 
			AND 
			p.Place=1 
			AND
			(
				SELECT COUNT(*)
				FROM OPENJSON(Pedig.Pedigree)
				WITH (
					Id INT,
					SD CHAR
				)
				WHERE Id = @FemaleId AND SD = 'D'
			) > 0
			ORDER BY Age DESC
		";

        public const string Get_SW_Offsprings_By_Sire = @"
			SELECT DISTINCT
				h.*,
				CONCAT(hg.Title, '-', ht.Name) MtDNATitle,
				hg.Color MtDNAColor,
				dbo.GetBestRaceClass(h.OId) BestRaceClass,
				hf.OId FatherOId,
				hf.[Name] FatherName,
				hm.OId MotherOId,
				hm.[Name] MotherName,
				hmf.OId BmFatherOId,
				hmf.[Name] BmFatherName
			FROM Horse h
			LEFT JOIN Coefficient c ON c.HorseOId=h.OId
			JOIN Position p ON p.HorseOId=h.OId AND p.Place=1
			LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
			LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
			-- Father
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father' 
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId 
			-- Mother
			LEFT JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother' 
			LEFT JOIN Horse hm ON hm.OId=rm.ParentOId 
			-- Father of Mother
			LEFT JOIN Relationship rmf ON rmf.HorseOId=hm.OId AND rmf.ParentType='Father' 
			LEFT JOIN Horse hmf ON hmf.OId=rmf.ParentOId 

			WHERE rf.ParentOId=(SELECT OId FROM Horse WHERE Id=@Id);
		";

        public const string Get_SW_Offsprings_By_BroodmareSire = @"
			SELECT 
				DISTINCT
				h.*,
				CONCAT(hg.Title, '-', ht.Name) MtDNATitle,
				hg.Color MtDNAColor,
				dbo.GetBestRaceClass(h.OId) BestRaceClass,
				hf.OId FatherOId,
				hf.[Name] FatherName,
				hm.OId MotherOId,
				hm.[Name] MotherName,
				hmf.OId BmFatherOId,
				hmf.[Name] BmFatherName
			FROM Horse h
			JOIN Position p ON p.HorseOId=h.OId AND p.Place=1
			LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
			LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
			
			-- Father
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father' 
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId 
			-- Mother
			LEFT JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother' 
			LEFT JOIN Horse hm ON hm.OId=rm.ParentOId 
			-- Father of Mother
			LEFT JOIN Relationship rmf ON rmf.HorseOId=hm.OId AND rmf.ParentType='Father' 
			LEFT JOIN Horse hmf ON hmf.OId=rmf.ParentOId 

			WHERE rmf.ParentOId=(SELECT OId FROM Horse WHERE Id=@Id);
		";

		public const string Get_Stake_Winners_By_Sire_Descendants = @"
			DECLARE @Id int = {0};
			DECLARE @DepthLevel int = 1;
			DECLARE @MaxGen int = {1};

			DECLARE @TblHorse AS Table
			(
				Id integer,
    			OId nvarchar(100),
    			IsTraversed bit,
				Depth integer
			)
 
			IF(@Id IS NOT NULL)
    			BEGIN
        			INSERT INTO @TblHorse(Id, OId, IsTraversed, Depth)
        			SELECT Id, OId, 0, @DepthLevel 
					FROM Horse
					WHERE Id=@Id
 		
        			WHILE((SELECT Count(1) FROM @TblHorse WHERE IsTraversed = 0) > 0)
        				BEGIN
							DECLARE @HorseId integer;
							DECLARE @HorseOId nvarchar(max) ;
							IF ((SELECT Count(1) FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel) > 0)
		    					SELECT TOP 1 @HorseId = Id, @HorseOId = OId, @DepthLevel = Depth FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel;
							ELSE
								BEGIN
									SET @DepthLevel = @DepthLevel + 1
									SELECT TOP 1 @HorseId = Id, @HorseOId = OId, @DepthLevel = Depth FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel;
								END

							IF (@DepthLevel >= @MaxGen) break;
							--IF ((SELECT Count(1) FROM @TblHorse) > 1000) break;

							INSERT INTO @TblHorse(Id, OId, IsTraversed, Depth)
            				SELECT h.Id, h.OId, 0, @DepthLevel +  1
            				FROM Horse h
							JOIN Relationship rf ON rf.HorseOId = h.OId AND rf.ParentType='Father'
							LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
            				WHERE h.isDeleted = 0 AND hf.OId = @HorseOId AND h.Sex = 'Male'
			
							UPDATE @TblHorse SET IsTraversed = 1 WHERE Id = @HorseId
        				END

        			SELECT DISTINCT h.Id, h.OId, h.Name,h.Age, h.Sex, h.Family, h.Country, h.MtDNA, CONCAT(hg.Title, '-', ht.[Name]) MtDNATitle, hg.Color MtDNAColor, dbo.GetBestRaceClass(h.OId) BestRaceClass, hf.Id FatherId, hf.OId FatherOId, hf.[Name] FatherName, hmf.Id BmFatherId, hmf.OId BmFatherOId, hmf.[Name] BmFatherName
        			FROM Horse h
					-- Father
					JOIN Relationship rf ON rf.HorseOId = h.OId AND rf.ParentType='Father'
					LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
					-- Mother
					LEFT JOIN Relationship rm ON rm.HorseOId = h.OId AND rm.ParentType='Mother'
					LEFT JOIN Horse hm ON hm.OId=rm.ParentOId
					-- Father of Mother
					LEFT JOIN Relationship rmf ON rmf.HorseOId = hm.OId AND rmf.ParentType='Father'
					LEFT JOIN Horse hmf ON hmf.OId=rmf.ParentOId
					-- MtDNA
					LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
					LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
					-- Position
					LEFT JOIN Position p ON p.HorseOId=h.OId
					WHERE h.Id IN (
						SELECT 
							h1.Id 
						FROM @TblHorse hf
						JOIN Relationship rf ON rf.ParentOId=hf.OId
						JOIN Horse h1 ON h1.OId=rf.HorseOId
					) AND h.isDeleted=0 AND p.Place=1
    			END
		";

        public const string Get_Stake_Winners_By_BroodmareSire_Descendants = @"
			DECLARE @Id int = {0} -- Subject male horse
			DECLARE @DepthLevel int = 1;
			DECLARE @MaxGen int = {1};

			DECLARE @TblHorse AS Table
			(
				Id integer,
    			OId nvarchar(100),
    			IsTraversed bit,
				Depth integer
			)
 
			IF(@Id IS NOT NULL)
    			BEGIN
        			INSERT INTO @TblHorse(Id, OId, IsTraversed, Depth)
        			SELECT Id, OId, 0, @DepthLevel 
					FROM Horse
					WHERE Id = @Id
 		
        			WHILE((SELECT Count(1) FROM @TblHorse WHERE IsTraversed = 0) > 0)
        				BEGIN
							DECLARE @HorseId integer;
							DECLARE @HorseOId nvarchar(max) ;
							IF ((SELECT Count(1) FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel) > 0)
		    					SELECT TOP 1 @HorseId = Id, @HorseOId = OId, @DepthLevel = Depth FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel;
							ELSE
								BEGIN
									SET @DepthLevel = @DepthLevel + 1
									SELECT TOP 1 @HorseId = Id, @HorseOId = OId, @DepthLevel = Depth FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel;
								END

							IF (@DepthLevel >= @MaxGen) break;
							--IF ((SELECT Count(1) FROM @TblHorse) > 1000) break;

							INSERT INTO @TblHorse(Id, OId, IsTraversed, Depth)
            				SELECT h.Id, h.OId, 0, @DepthLevel +  1
            				FROM Horse h
							-- Father
							JOIN Relationship rf ON rf.HorseOId = h.OId AND rf.ParentType='Father'
							LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
            				WHERE h.isDeleted = 0 AND hf.OId = @HorseOId AND h.Sex = 'Male'
			
							UPDATE @TblHorse SET IsTraversed = 1 WHERE Id = @HorseId
        				END

					SELECT DISTINCT h.Id, h.OId, h.Name,h.Age, h.Sex, h.Family, h.Country, h.MtDNA, CONCAT(hg.Title, '-', ht.[Name]) MtDNATitle, hg.Color MtDNAColor, dbo.GetBestRaceClass(h.OId) BestRaceClass, hf.Id FatherId, hf.OId FatherOId, hf.[Name] FatherName, hmf.Id BmFatherId, hmf.OId BmFatherOId, hmf.[Name] BmFatherName
        			FROM Horse h
					-- Father
					JOIN Relationship rf ON rf.HorseOId = h.OId AND rf.ParentType='Father'
					LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
					-- Mother
					LEFT JOIN Relationship rm ON rm.HorseOId = h.OId AND rm.ParentType='Mother'
					LEFT JOIN Horse hm ON hm.OId=rm.ParentOId
					-- Father of Mother
					LEFT JOIN Relationship rmf ON rmf.HorseOId = hm.OId AND rmf.ParentType='Father'
					LEFT JOIN Horse hmf ON hmf.OId=rmf.ParentOId
					-- MtDNA
					LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
					LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
					-- Position
					LEFT JOIN Position p ON p.HorseOId=h.OId
					WHERE h.Id IN (
						SELECT 
							h.Id 
						FROM @TblHorse hf
						JOIN Relationship rf ON rf.ParentOId=hf.OId
						JOIN Horse hm ON hm.OId=rf.HorseOId AND hm.Sex='Female'		
						JOIN Relationship rm ON rm.ParentOId=hm.OId
						JOIN Horse h ON h.OId=rm.HorseOId
					) AND h.isDeleted=0 AND p.Place=1
        
    			END
		";

		public const string Get_Parent_By_Type = @"
			SELECT
				h.*
			FROM Horse h 
			JOIN Relationship r ON r.ParentOId=h.OId AND r.ParentType='{0}'
			WHERE r.HorseOId=(SELECT OId FROM Horse WHERE Id={1})
		";

		public const string Get_Stake_Winners_By_Wildcard1 = @"
			DECLARE @Horse1Id INT = {0};
			DECLARE @Horse2Id INT = {1};

			SELECT
				h.Id, 
				h.OId, 
				h.[Name], 
				h.Age, 
				h.Sex, 
				h.Country,
				h.Family, 
				h.MtDNA, 
				CONCAT(hg.Title, '-', ht.[Name]) MtDNATitle,
				hg.Color MtDNAColor, 
				dbo.GetBestRaceClass(h.OId) BestRaceClass,
				hf.OId FatherOId,
				hf.[Name] FatherName,
				hm.OId MotherOId,
				hm.[Name] MotherName
			FROM
				Horse h
			-- Father
			LEFT JOIN Relationship rf ON rf.HorseOId = h.OId AND rf.ParentType='Father'
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
			-- Mother
			LEFT JOIN Relationship rm ON rm.HorseOId = h.OId AND rm.ParentType='Mother'
			LEFT JOIN Horse hm ON hm.OId=rm.ParentOId
			-- MtDNA
			LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
			LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId

			WHERE h.OId IN (
				SELECT HorseOId
				FROM Pedig
				WHERE HorseOId IN (
					-- Search SW with Name 1 in FATHER side on 5x pedigree
					SELECT DISTINCT Pedig.HorseOId
					FROM Pedig
					JOIN Position ON Position.HorseOId=Pedig.HorseOId AND Position.Place=1
					WHERE EXISTS
					(
						SELECT *
						FROM OPENJSON(Pedig.Pedigree)
						WITH (
							Id INT,
							SD CHAR
						)
						WHERE Id = @Horse1Id AND SD = 'S'
					)
				) 
				AND EXISTS
				(
					-- Name 2 search in MOTHER side
					SELECT *
					FROM OPENJSON(Pedig.Pedigree)
					WITH (
						Id INT,
						SD CHAR
					)
					WHERE Id = @Horse2Id AND SD = 'D'
				)
			)
		";

		public static string Get_Stake_Winners_By_Wildcard2(int horse1Id, int? horse2Id, int horse3Id, int? horse4Id)
        {
			string query = @"
					SELECT DISTINCT
						h.Id, 
						h.OId, 
						h.[Name], 
						h.Age, 
						h.Sex, 
						h.Country,
						h.Family, 
						h.MtDNA, 
						CONCAT(hg.Title, '-', ht.[Name]) MtDNATitle,
						hg.Color MtDNAColor,
						dbo.GetBestRaceClass(h.OId) BestRaceClass,
						hf.OId FatherOId,
						hf.[Name] FatherName,
						hm.OId MotherOId,
						hm.[Name] MotherName
					FROM
						Horse h
					--Father
					LEFT JOIN Relationship rf ON rf.HorseOId = h.OId AND rf.ParentType = 'Father'
					LEFT JOIN Horse hf ON hf.OId = rf.ParentOId
					-- Mother
					LEFT JOIN Relationship rm ON rm.HorseOId = h.OId AND rm.ParentType = 'Mother'
					LEFT JOIN Horse hm ON hm.OId = rm.ParentOId
					-- MtDNA
					LEFT JOIN HaploType ht ON ht.Id = h.MtDNA
					LEFT JOIN HaploGroup hg ON hg.Id = ht.GroupId

					WHERE h.OId IN (
						SELECT HorseOId
						FROM Pedig
						WHERE HorseOId IN (
							--Search SW with Name 1, Name2 in FATHER side on 5x pedigree
							SELECT DISTINCT Pedig.HorseOId
							FROM Pedig
							JOIN Position ON Position.HorseOId = Pedig.HorseOId AND Position.Place = 1
							WHERE {0}
						)
						-- Name3 and Name4 search in MOTHER side
						AND ({1})
					)
				";

			var where1 = "";
			if (horse1Id == horse2Id)
            {
                where1 = string.Format(@"
					(
						SELECT COUNT(*)
						FROM OPENJSON(Pedig.Pedigree)
						WITH(
							Id INT,
							SD CHAR
						)
						WHERE Id = {0} AND SD = 'S'
					) > 1", horse1Id);
            } 
			else
            {
                where1 = string.Format(@"
					(
						SELECT COUNT(*)
						FROM OPENJSON(Pedig.Pedigree)
						WITH (
							Id INT,
							SD CHAR
						)
						WHERE Id = {0} AND SD = 'S'
					) > 0", horse1Id);

				if (horse2Id != null)
                {
                    where1 += string.Format(@"
							AND
						(
							SELECT COUNT(*)
							FROM OPENJSON(Pedig.Pedigree)
							WITH(
								Id INT,
								SD CHAR
							)
							WHERE Id = {0} AND SD = 'S'
						) > 0", horse2Id);
                }
            }

			var where2 = "";
			if (horse3Id == horse4Id)
            {
                where2 = string.Format(@"
					(
						SELECT COUNT(*)
						FROM OPENJSON(Pedig.Pedigree)
						WITH (
							Id INT,
							SD CHAR
						)
						WHERE Id = {0} AND SD = 'D'
					) > 1
				", horse3Id);
            }
			else
            {
                where2 = string.Format(@"
					(
						SELECT COUNT(*)
						FROM OPENJSON(Pedig.Pedigree)
						WITH (
							Id INT,
							SD CHAR
						)
						WHERE Id = {0} AND SD = 'D'
					) > 0 ", horse3Id);

				if (horse4Id != null)
                {
                    where2 += string.Format(@"
					 AND
					(
						SELECT COUNT(*)
						FROM OPENJSON(Pedig.Pedigree)
						WITH (
							Id INT,
							SD CHAR
						)
						WHERE Id = {0} AND SD = 'D'
					) > 0
				", horse4Id);
                }
            }

			return string.Format(query, where1, where2);
		}

		public const string Mordern_Horses = @"
			SELECT
				h.*,
				dbo.GetBestRaceClass(h.OId) BestRaceClass,
				c.HistoricalBPR,
				c.ZHistoricalBPR
			FROM Horse h
			LEFT JOIN Coefficient c ON c.HorseOId=h.OId
			WHERE h.isDeleted=0 AND h.Age >= 1991
		";

        public const string Mordern_Horses_SW = @"
			SELECT
				h.*,
				dbo.GetBestRaceClass(h.OId) BestRaceClass,
				c.HistoricalBPR,
				c.ZHistoricalBPR
			FROM Horse h
			LEFT JOIN Coefficient c ON c.HorseOId=h.OId
			LEFT JOIN Position p ON p.HorseOId=h.OId
			WHERE h.isDeleted=0 AND h.Age >= 1991 AND p.
		";

		public static string Get_Stake_Winners_By_Wildcard_Query_Position(Dictionary<int, int> searches)
        {
			var query = @"
						SELECT
							h.Id, 
							h.OId, 
							h.[Name], 
							h.Age, 
							h.Sex, 
							h.Country,
							h.Family, 
							h.MtDNA, 
							CONCAT(hg.Title, '-', ht.[Name]) MtDNATitle,
							hg.Color MtDNAColor, 
							dbo.GetBestRaceClass(h.OId) BestRaceClass,
							hf.OId FatherOId,
							hf.[Name] FatherName,
							hm.OId MotherOId,
							hm.[Name] MotherName
						FROM
							Horse h
						-- Father
						LEFT JOIN Relationship rf ON rf.HorseOId = h.OId AND rf.ParentType='Father'
						LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
						-- Mother
						LEFT JOIN Relationship rm ON rm.HorseOId = h.OId AND rm.ParentType='Mother'
						LEFT JOIN Horse hm ON hm.OId=rm.ParentOId
						-- MtDNA
						LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
						LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId

						WHERE h.OId IN (
							SELECT DISTINCT Pedig.HorseOId
							FROM Pedig
							JOIN Position ON Position.HorseOId=Pedig.HorseOId AND Position.Place=1
							WHERE 
							{0}
						)
			";

			var where = "";
            foreach (var item in searches)
            {
				if (where != "") where += " AND ";
				where += string.Format(@"
							EXISTS
							(
								SELECT *
								FROM OPENJSON(Pedig.Pedigree)
								WITH (
									Id INT,
									Seq INT
								)
								WHERE (Id={0} AND Seq={1}) 
							)", item.Value, item.Key+1);
            }

			return string.Format(query, where);
        }

		public const string Update_Pedigree = @"
			DECLARE @Id INT = {0};
			DECLARE @OId NVARCHAR(100);

			SELECT @OId=OId FROM Horse WHERE Id=@Id

			IF (SELECT COUNT(*) FROM Pedig WHERE HorseOId=@OId) > 0
				UPDATE Pedig SET Pedigree=(SELECT * FROM GetFullPedigree(@Id, 5) FOR JSON AUTO), PedigreeUpdatedAt=CURRENT_TIMESTAMP WHERE HorseOId=@OId
			ELSE
				INSERT INTO Pedig (HorseOId, Pedigree, PedigreeUpdatedAt, CreatedAt) VALUES (@OId, (SELECT * FROM GetFullPedigree(@Id, 5) FOR JSON AUTO), CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
		";

        public const string Update_ProbOrigs = @"
			DECLARE @Id INT = {0};
			DECLARE @ProbOrigs NVARCHAR(MAX) = '{1}';
			DECLARE @OId NVARCHAR(100);

			SELECT @OId=OId FROM Horse WHERE Id=@Id

			IF (SELECT COUNT(*) FROM Pedig WHERE HorseOId=@OId) > 0
				UPDATE Pedig SET ProbOrigs=@ProbOrigs, ProbOrigsUpdatedAt=CURRENT_TIMESTAMP WHERE HorseOId=@OId
			ELSE
				INSERT INTO Pedig (HorseOId, ProbOrigs, ProbOrigsUpdatedAt, CreatedAt) VALUES (@OId, @ProbOrigs, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
		";

		public const string Delete_ProbOrigs = @"
			UPDATE Pedig SET ProbOrigs=NULL, ProbOrigsUpdatedAt=NULL WHERE HorseOId NOT IN (SELECT h.OId FROM Horse h LEFT JOIN Coefficient c ON c.HorseOId=h.OId WHERE c.ZHistoricalBPR IS NOT NULL AND c.Pedigcomp>=95 AND h.isDeleted=0)
		";

        public const string Delete_GRain_Data = @"
			UPDATE Coefficient SET Bal=NULL, AHC=NULL, Kal=NULL, GRainProcessStartedAt=NULL, GRainUpdatedAt=NULL WHERE HorseOId NOT IN (SELECT h.OId FROM Horse h LEFT JOIN Coefficient c ON c.HorseOId=h.OId WHERE c.ZCurrentBPR IS NOT NULL AND c.Pedigcomp>=95 AND h.isDeleted=0);
		";


        public const string Get_Stake_Winners_By_Mare_Descendants = @"
			DECLARE @Id int = {0};
			DECLARE @MaxGen int = {1};
			DECLARE @DepthLevel int = 1;

			DECLARE @TblHorse AS Table
			(
				Id integer,
    			OId nvarchar(100),
				IsTraversed bit,
				Depth integer
			)
 
			IF(@Id IS NOT NULL)
    			BEGIN
        			INSERT INTO @TblHorse(Id, OId, IsTraversed, Depth)
        			SELECT h.Id, h.OId, (CASE WHEN h.Sex='Male' THEN 1 ELSE 0 END), @DepthLevel
        			FROM Horse h
					JOIN Relationship r ON r.HorseOId = h.OId
        			WHERE h.isDeleted = 0 AND r.ParentOId = (SELECT OId FROM Horse WHERE Id=@Id) AND (h.Sex = 'Female' OR h.Sex = 'Male' AND ((SELECT COUNT(*) FROM Relationship WHERE ParentOId=h.OId)>0 OR (SELECT COUNT(*) FROM Position WHERE HorseOId=h.OId AND Place=1)>0))
 		
        			WHILE((SELECT Count(1) FROM @TblHorse WHERE IsTraversed = 0) > 0)
        				BEGIN
							DECLARE @HorseId integer;
							DECLARE @HorseOId nvarchar(max) ;
							IF ((SELECT Count(1) FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel) > 0)
		    					SELECT TOP 1 @HorseId = Id, @HorseOId = OId, @DepthLevel = Depth FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel;
							ELSE
								BEGIN
									SET @DepthLevel = @DepthLevel + 1
									SELECT TOP 1 @HorseId = Id, @HorseOId = OId, @DepthLevel = Depth FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel;
								END

							IF (@DepthLevel >= @MaxGen) break;
							--IF ((SELECT Count(1) FROM @TblHorse) > 1000) break;

							INSERT INTO @TblHorse(Id, OId, IsTraversed, Depth)
            				SELECT h.Id, h.OId, (CASE WHEN h.Sex='Male' THEN 1 ELSE 0 END), @DepthLevel +  1
            				FROM Horse h
							JOIN Relationship r ON r.HorseOId = h.OId
            				WHERE h.isDeleted = 0 AND r.ParentOId = @HorseOId AND (h.Sex = 'Female' OR h.Sex = 'Male' AND ((SELECT COUNT(*) FROM Relationship WHERE ParentOId=h.OId)>0 OR (SELECT COUNT(*) FROM Position WHERE HorseOId=h.OId AND Place=1)>0))
			
							UPDATE @TblHorse SET IsTraversed = 1 WHERE Id = @HorseId
        				END

        			SELECT DISTINCT h.Id, h.OId, h.Name, h.Age, h.Sex, h.Family, h.Country, h.MtDNA, CONCAT(hg.Title, '-', ht.[Name]) MtDNATitle, hg.Color MtDNAColor, dbo.GetBestRaceClass(h.OId) BestRaceClass, hf.Id FatherId, hf.OId FatherOId, hf.[Name] FatherName, hm.Id MotherId, hm.OId MotherOId, hm.[Name] MotherName, th.Depth
        			FROM Horse h
					JOIN @TblHorse th ON th.Id=h.Id
					-- Father
					JOIN Relationship rf ON rf.HorseOId = h.OId AND rf.ParentType='Father'
					LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
					-- Mother
					LEFT JOIN Relationship rm ON rm.HorseOId = h.OId AND rm.ParentType='Mother'
					LEFT JOIN Horse hm ON hm.OId=rm.ParentOId
					-- MtDNA
					LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
					LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
					WHERE h.isDeleted=0
    			END
		";


		public const string Get_Horses_For_Female_Line = @"
			DECLARE @Id int = {0};
			DECLARE @MaxGen int = 4;
			DECLARE @DepthLevel int = 0;

			DECLARE @TblHorse AS Table
			(
				Id integer,
    			OId nvarchar(100),
				IsTraversed bit,
				Depth integer
			)
 
			IF(@Id IS NOT NULL)
    			BEGIN
        			INSERT INTO @TblHorse(Id, OId, IsTraversed, Depth)
        			SELECT h.Id, h.OId, 0, @DepthLevel
        			FROM Horse h
        			WHERE h.isDeleted = 0 AND h.Id=@Id
 		
        			WHILE((SELECT Count(1) FROM @TblHorse WHERE IsTraversed = 0) > 0)
        				BEGIN
							DECLARE @HorseId integer;
							DECLARE @HorseOId nvarchar(max) ;
							IF ((SELECT Count(1) FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel) > 0)
		    					SELECT TOP 1 @HorseId = Id, @HorseOId = OId, @DepthLevel = Depth FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel;
							ELSE
								BEGIN
									SET @DepthLevel = @DepthLevel + 1
									SELECT TOP 1 @HorseId = Id, @HorseOId = OId, @DepthLevel = Depth FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel;
								END

							IF (@DepthLevel >= @MaxGen) break;
							--IF ((SELECT Count(1) FROM @TblHorse) > 1000) break;

							INSERT INTO @TblHorse(Id, OId, IsTraversed, Depth)
            				SELECT h.Id, h.OId, (CASE WHEN h.Sex='Male' THEN 1 ELSE 0 END), @DepthLevel +  1
            				FROM Horse h
							JOIN Relationship r ON r.HorseOId = h.OId
            				WHERE h.isDeleted = 0 AND r.ParentOId = @HorseOId
			
							UPDATE @TblHorse SET IsTraversed = 1 WHERE Id = @HorseId
        				END

        			SELECT h.Id, h.OId, h.Name, h.Age, h.Sex, h.Family, h.Country, h.MtDNA, CONCAT(hg.Title, '-', ht.[Name]) MtDNATitle, hg.Color MtDNAColor, dbo.GetBestRaceClass(h.OId) BestRaceClass, hf.Id FatherId, hf.OId FatherOId, hf.[Name] FatherName, hm.Id MotherId, hm.OId MotherOId, hm.[Name] MotherName, th.Depth
        			FROM Horse h
					JOIN @TblHorse th ON th.Id=h.Id
					-- Father
					JOIN Relationship rf ON rf.HorseOId = h.OId AND rf.ParentType='Father'
					LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
					-- Mother
					LEFT JOIN Relationship rm ON rm.HorseOId = h.OId AND rm.ParentType='Mother'
					LEFT JOIN Horse hm ON hm.OId=rm.ParentOId
					-- MtDNA
					LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
					LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
					WHERE h.isDeleted=0-- AND p.Place=1
					ORDER BY th.Depth
    			END
		";

		public const string Horse_Races_SW = @"
			SELECT 
				p.HorseOId,
				p.Place,
				r.Id RaceId, 
				r.[Name],
				r.[Date],
				r.Country,
				r.Distance,
				r.Surface,
				r.[Type],
				r.[Status],
				h.MtDNA
			FROM Position p 
			JOIN Race r ON r.Id=p.RaceId
			JOIN Horse h ON h.OId=p.HorseOId
			WHERE p.Place=1
		";


		public const string Get_Pedigs_For_ProbOrigs = @"
			SELECT HorseOId, ProbOrigs FROM Pedig WHERE ProbOrigsUpdatedAt IS NOT NULL AND ProbOrigs IS NOT NULL
		";

		public const string Get_ML_Model_Data = @"
			DECLARE @now DATETIME = GETDATE();

			SELECT
			--h.[Name],

			--mtDNA data
			h.MtDNA as mtDNAGroup, 
			-- need to concatenate Haplogroups of 8 great grandparents of subject horse
			-- so in this case we would be looking for the haplogroup of Danehill, Offshore Boom, Key of Luck, Elas Gold, Montjeu, Out West, Zilzal and Willowy Mood
			-- Concatenate as a single string of their ID values on dbo.Haplogroup
			-- So Danehill is L (12), Offshore Boom is L (12), Key Of Luck is N (14), Elas Gold is I (9), Montjeu is N (14), Out West is L (12), Zilzal is L(12) and Willowy Mood is G(7)
			-- so the Concatenate is 12121491412127
			-- each horse has a concatenate string of haplogroup. Should we store this and set up a CRON when it does the Coefficient of Inbreeding calcs?
			-- for UNK mtDNAhaplogroup, use the number 0
			CONCAT(ISNULL(tfff.Name, 'UNK'), ISNULL(tffm.Name, 'UNK'), ISNULL(tfmf.Name, 'UNK'), ISNULL(tfmm.Name, 'UNK'), ISNULL(tmff.Name, 'UNK'), ISNULL(tmfm.Name, 'UNK'), ISNULL(tmmf.Name, 'UNK'), ISNULL(tmmm.Name, 'UNK')) MtDNAGroups,
			--inbreeding of individual and ancestry
			c.COI, --subject Horse
			c.COI1, -- Sire/BroodmareSire
			c.COI2, -- Sire/Granddam
			c.COI3, -- Dam/Grandsire
			c.COI4, -- Dam/DamofSire
			c.COI5, -- Grandsire/BroodmareSire
			c.COI6, -- Grandsire/Granddam
			c.COI7, -- DamofSire/BroodmareSire
			c.COI8, -- DamofSire/Granddam
			c.COID1, -- Horse/Sire Diff
			c.COID2, -- Horse/Dam Diff
			c.COID3, -- Horse/GrandSire
			c.COID4, -- Horse/DamofSire
			c.COID5, -- Horse/BroodmareSire
			c.COID6, -- Horse/Granddam

			-- PedigreeData
			c.GI, -- generational Interval
			c.GDGS, -- Distance to Grandsire
			c.GDGD, -- Distance to Granddam
			c.GSSD, -- Distance to Sire of Dam
			c.GSDD, -- Distance to Broodmare Sire
			--- need variable Count of Unique Ancestors (as UniqueAncestors) ---
			c.UniqueAncestorsCount,
			c.AHC, -- AHC of Horse
			c.Bal, -- Ballou of horse
			c.Kal, --Kalinowski of horse
			c.Pedigcomp, -- Pedigree Completeness

			-- Sire/Father Data
				-- Sire Name (in this case it would be Society Rock)
				--hf.[Name] SireName,
				-- HistoricalBPR of father as a runner
				cf.HistoricalBPR SireHistoricalBPR,
				-- ZHistoricalBRP of father as a runner
				cf.ZHistoricalBPR SireZHistoricalBPR,
				-- COI of Sire
				cf.COI SireCOI,
				-- AHC of Sire
				cf.AHC SireAHC,
				-- Bal of Sire
				cf.Bal SireBal,
				-- Kal of Sire
				cf.Kal SireKal,
				-- Historical Stallion Rating of Sire (SR Historical)
				sf.HistoricalStallionRating SireHistoricalSR,
				-- IV, A/E, PRB2 values
				sf.CurrentIV SireCurrentIV, sf.CurrentAE SireCurrentAE, sf.CurrentPRB2 SireCurrentPRB2,
				sf.HistoricalIV SireHistoricalIV, sf.HistoricalAE SireHistoricalAE, sf.HistoricalPRB2 SireHistoricalPRB2,
	
			-- Dam/Mother Data
				-- Dam Name (in this case it would be Motion Lass)
				--hm.[Name] DamName,
				-- HistoricalBPR of mother as a runner
				cm.HistoricalBPR DamHistoricalBPR,
				-- ZHistoricalBRP of mother as a runner
				cm.ZHistoricalBPR DamZHistoricalBPR,
				-- COI of Dam
				cm.COI DamCOI,
				-- AHC of Dam
				cm.AHC DamAHC,
				-- Bal of Dam
				cm.Bal DamBal,
				-- Kal of Dam
				cm.Kal DamKal,

			-- GrandSire Data (father of father)
				-- GrandSire Name (in this case it would be Rock of Gibraltar)
				--hff.[Name] GrandSireName,
				-- HistoricalBPR of Grandsire as a runner
				cff.HistoricalBPR GrandSireHistoricalBPR,
				-- ZHistoricalBRP of GrandSire as a runner
				cff.ZHistoricalBPR GrandSireZHistoricalBPR,
				-- COI of GrandSire
				cff.COI GrandSireCOI,
				-- AHC of GrandSire
				cff.AHC GrandSireAHC,
				-- Bal of GrandSire
				cff.Bal GrandSireBal,
				-- Kal of GrandSire
				cff.Kal GrandSireKal,
				-- Historical Stallion Rating of GrandSire (SR Historical for Rock of Gibraltar)
				sff.HistoricalStallionRating GrandSireHistoricalSR,
				-- Historical Sire of Sire Rating of Grandsire (Historical SOS SR for Rock of Gribraltar)
				sff.SOSHistoricalStallionRating GrandSireSOSHistoricalSR,

			-- BroodmareSire Data (father of mother)
				-- BroodmareSire Name (in this case it would be Motivator)
				--hmf.[Name] BroodmareSireName,
				-- HistoricalBPR of Broodmaresire as a runner
				cmf.HistoricalBPR BroodmareSireHistoricalBPR,
				-- ZHistoricalBRP of BroodmareSire as a runner
				cmf.ZHistoricalBPR BroodmareSireZHistoricalBPR,
				-- COI of Broodmaresire
				cmf.COI BroodmareSireCOI,
				-- AHC of Broodmaresire
				cmf.AHC BroodmareSireAHC,
				-- Bal of Broodmaresire
				cmf.Bal BroodmareSireBal,
				-- Kal of Broodmaresire of Sire
				cmf.Kal BroodmareSireKal,
				-- Historical Stallion Rating of BroodmareSire (SR Historical for Motivator)
				smf.HistoricalStallionRating BroodmareSireHistoricalSR,
				-- Historical Broodmare Sire Rating of Broodmaresire (Historical BMS SR for Motivator)
				smf.BMShistoricalStallionRating BroodmareSireBMSHIstoricalSR,

			-- BroodmareSire of Sire Data (father of mother of father)
				-- BroodmareSire Name (in this case it would be Key of Luck)
				--hfmf.[Name] BroodmareSireOfSireName,
				-- HistoricalBPR of Broodmaresire of Sire as a runner
				cfmf.HistoricalBPR BroodmareSireOfSireHistoricalBPR,
				-- ZHistoricalBRP of Broodmaresire of Sire as a runner
				cfmf.ZHistoricalBPR BroodmareSireOfSireZHistoricalBPR,
				-- COI of Broodmaresire of Sire
				cfmf.COI BroodmareSireOfSireCOI,
				-- AHC of Broodmaresire of Sire
				cfmf.AHC BroodmareSireOfSireAHC,
				-- Bal of Broodmaresire of Sire
				cfmf.Bal BroodmareSireOfSireBal,
				-- Kal of Broodmaresire of Sire
				cfmf.Kal BroodmareSireOfSireKal,
				-- Historical Stallion Rating of BroodmareSire of Sire (SR Historical for Key of Luck)
				sfmf.HistoricalStallionRating BroodmareSireOfSireHistoricalSR,
				-- Historical Broodmare Sire of Sire Rating of Broodmaresire (Historical BMSOS SR for Key of Luck)
				sfmf.BMSHistoricalStallionRating BroodmareSireOfSireBMSHistoricalSR,

			-- Granddam Data (mother of mother)
				-- GrandDam Name (in this case it would be Tarneem)
				--hmm.[Name] GrandDamName,
				-- HistoricalBPR of Granddam as a runner
				cmm.HistoricalBPR GrandDamHistoricalBPR,
				-- ZHistoricalBRP of Granddam as a runner
				cmm.ZHistoricalBPR GrandDamZHistoricalBPR,
				-- COI of GrandDam
				cmm.COI GrandDamCOI,
				-- AHC of GrandDam
				cmm.AHC GrandDamAHC,
				-- Bal of GrandDam
				cmm.Bal GrandDamBal,
				-- Kal of GrandDam
				cmm.Kal GrandDamKal,

			-- Target variable to predict
			c.ZCurrentBPR as [Target]

			FROM Horse h
			JOIN Coefficient as c ON c.HorseOId = h.OId

			-- Father
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
			-- Father of Father
			LEFT JOIN Relationship rff ON rff.HorseOId=rf.ParentOId AND rff.ParentType='Father'
			LEFT JOIN Horse hff ON hff.OId=rff.ParentOId
			-- Mother of Father
			LEFT JOIN Relationship rfm ON rfm.HorseOId=rf.ParentOId AND rfm.ParentType='Mother'
			LEFT JOIN Horse hfm ON hfm.OId=rfm.ParentOId
			-- Father of Father of Father
			LEFT JOIN Relationship rfff ON rfff.HorseOId=rff.ParentOId AND rfff.ParentType='Father'
			LEFT JOIN Horse hfff ON hfff.OId=rfff.ParentOId
			-- Mother of Father of Father
			LEFT JOIN Relationship rffm ON rffm.HorseOId=rff.ParentOId AND rffm.ParentType='Mother'
			LEFT JOIN Horse hffm ON hffm.OId=rffm.ParentOId
			-- Father of Mother of Father
			LEFT JOIN Relationship rfmf ON rfmf.HorseOId=rfm.ParentOId AND rfmf.ParentType='Father'
			LEFT JOIN Horse hfmf ON hfmf.OId=rfmf.ParentOId
			-- Mother of Mother of Father
			LEFT JOIN Relationship rfmm ON rfmm.HorseOId=rfm.ParentOId AND rfmm.ParentType='Mother'
			LEFT JOIN Horse hfmm ON hfmm.OId=rfmm.ParentOId

			-- Mother
			LEFT JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother'
			LEFT JOIN Horse hm ON hm.OId=rm.ParentOId
			-- Father of Mother
			LEFT JOIN Relationship rmf ON rmf.HorseOId=rm.ParentOId AND rmf.ParentType='Father'
			LEFT JOIN Horse hmf ON hmf.OId=rmf.ParentOId
			-- Mother of Mother
			LEFT JOIN Relationship rmm ON rmm.HorseOId=rm.ParentOId AND rmm.ParentType='Mother'
			LEFT JOIN Horse hmm ON hmm.OId=rmm.ParentOId
			-- Father of Father of Mother
			LEFT JOIN Relationship rmff ON rmff.HorseOId=rmf.ParentOId AND rmff.ParentType='Father'
			LEFT JOIN Horse hmff ON hmff.OId=rmff.ParentOId
			-- Mother of Father of Mother
			LEFT JOIN Relationship rmfm ON rmfm.HorseOId=rmf.ParentOId AND rmfm.ParentType='Mother'
			LEFT JOIN Horse hmfm ON hmfm.OId=rmfm.ParentOId
			-- Father of Mother of Mother
			LEFT JOIN Relationship rmmf ON rmmf.HorseOId=rmm.ParentOId AND rmmf.ParentType='Father'
			LEFT JOIN Horse hmmf ON hmmf.OId=rmmf.ParentOId
			-- Mother of Mother of Mother
			LEFT JOIN Relationship rmmm ON rmmm.HorseOId=rmm.ParentOId AND rmmm.ParentType='Mother'
			LEFT JOIN Horse hmmm ON hmmm.OId=rmmm.ParentOId

			-- Coefficients
			LEFT JOIN Coefficient cf ON cf.HorseOId=hf.OId
			LEFT JOIN Coefficient cm ON cm.HorseOId=hm.OId
			LEFT JOIN Coefficient cff ON cff.HorseOId=hff.OId
			LEFT JOIN Coefficient cmf ON cmf.HorseOId=hmf.OId
			LEFT JOIN Coefficient cfmf ON cfmf.HorseOId=hfmf.OId
			LEFT JOIN Coefficient cmm ON cmm.HorseOId=hmm.OId

			-- StallionRatings
			LEFT JOIN StallionRating sf ON sf.HorseOId=hf.OId
			LEFT JOIN StallionRating sff ON sff.HorseOId=hff.OId
			LEFT JOIN StallionRating smf ON smf.HorseOId=hmf.OId
			LEFT JOIN StallionRating sfmf ON sfmf.HorseOId=hfmf.OId

			-- HaploGroups
			LEFT JOIN HaploType tfff ON tfff.Id=hfff.MtDNA LEFT JOIN HaploGroup gfff ON gfff.Id=tfff.GroupId
			LEFT JOIN HaploType tffm ON tffm.Id=hffm.MtDNA LEFT JOIN HaploGroup gffm ON gffm.Id=tffm.GroupId
			LEFT JOIN HaploType tfmf ON tfmf.Id=hfmf.MtDNA LEFT JOIN HaploGroup gfmf ON gfmf.Id=tfmf.GroupId
			LEFT JOIN HaploType tfmm ON tfmm.Id=hfmm.MtDNA LEFT JOIN HaploGroup gfmm ON gfmm.Id=tfmm.GroupId
			LEFT JOIN HaploType tmff ON tmff.Id=hmff.MtDNA LEFT JOIN HaploGroup gmff ON gmff.Id=tmff.GroupId
			LEFT JOIN HaploType tmfm ON tmfm.Id=hmfm.MtDNA LEFT JOIN HaploGroup gmfm ON gmfm.Id=tmfm.GroupId
			LEFT JOIN HaploType tmmf ON tmmf.Id=hmmf.MtDNA LEFT JOIN HaploGroup gmmf ON gmmf.Id=tmmf.GroupId
			LEFT JOIN HaploType tmmm ON tmmm.Id=hmmm.MtDNA LEFT JOIN HaploGroup gmmm ON gmmm.Id=tmmm.GroupId

			WHERE h.Id IN (SELECT Id FROM GetMLGroup(@now)) AND c.Pedigcomp >= 95;
		";

		public static string Search_Horses_Ex(string q, string sex)
        {
			var query = @"
				SELECT 
					h.Id, 
					h.OId, 
					h.[Name], 
					h.Sex, 
					h.Age,
					h.Family, 
					CONCAT(hg.Title, '-', ht.Name) MtDNATitle,
					hg.Color MtDNAColor,
					h.Country,
					dbo.GetBestRaceClass(h.OId) BestRaceClass,
					hf.OId AS FatherOId,
					hf.Name AS FatherName,
					hm.OId AS MotherOId,
					hm.Name AS MotherName 
				FROM Horse h 
				LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father' 
				LEFT JOIN Horse hf ON hf.OId=rf.ParentOId 
				LEFT JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother' 
				LEFT JOIN Horse hm ON hm.OId=rm.ParentOId 
				LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
				LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
				WHERE {0}
			";

			var where = $"h.isDeleted = 0 AND h.[Name] LIKE '{q}'";
			if (sex != null) where += $" AND h.Sex='{sex}'";

			return string.Format(query, where);
        }

		public const string Get_MtDNA_Flags = @"
			SELECT
				f.Id, 
				hs.Id startHorseId,
				hs.OId startHorseOId,
				hs.[Name] startHorseName,
				hs.Age startHorseAge,
				hs.Country startHorseCountry,
				he.Id endHorseId,
				he.OId endHorseOId,
				he.[Name] endHorseName,
				he.Age endHorseAge,
				he.Country endHorseCountry
			FROM MtDNAFlags f
			LEFT JOIN Horse hs ON f.StartHorseOId=hs.OId
			LEFT JOIN Horse he ON f.EndHorseOId=he.OId
		";

		public const string Set_MtDNA_Flags = @"
			DECLARE @StartHorseOId nvarchar(50) = '{0}';
			DECLARE @EndHorseOId nvarchar(50) = '{1}';
			DECLARE @MaxGen int = 100;
			DECLARE @DepthLevel int = 1;

			DECLARE @TblHorse AS Table
			(
				Id integer,
    			OId nvarchar(100),
				IsTraversed bit,
				Depth integer
			)

			BEGIN
        			INSERT INTO @TblHorse(Id, OId, IsTraversed, Depth)
        			SELECT h.Id, h.OId, (CASE WHEN h.Sex='Male' THEN 1 ELSE 0 END), @DepthLevel
        			FROM Horse h
					JOIN Relationship r ON r.HorseOId = h.OId
        			WHERE h.isDeleted = 0 AND r.ParentOId = @StartHorseOId
 		
        			WHILE((SELECT Count(1) FROM @TblHorse WHERE IsTraversed = 0) > 0)
        				BEGIN
							DECLARE @HorseId integer;
							DECLARE @HorseOId nvarchar(max) ;
							IF ((SELECT Count(1) FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel) > 0)
		    					SELECT TOP 1 @HorseId = Id, @HorseOId = OId, @DepthLevel = Depth FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel;
							ELSE
								BEGIN
									SET @DepthLevel = @DepthLevel + 1
									SELECT TOP 1 @HorseId = Id, @HorseOId = OId, @DepthLevel = Depth FROM @TblHorse WHERE IsTraversed = 0 AND Depth = @DepthLevel;
								END

							IF (@DepthLevel >= @MaxGen) break;

							INSERT INTO @TblHorse(Id, OId, IsTraversed, Depth)
            				SELECT h.Id, h.OId, (CASE WHEN h.Sex='Male' OR h.OId=@EndHorseOId THEN 1 ELSE 0 END), @DepthLevel +  1
            				FROM Horse h
							JOIN Relationship r ON r.HorseOId = h.OId
            				WHERE h.isDeleted = 0 AND r.ParentOId = @HorseOId
			
							UPDATE @TblHorse SET IsTraversed = 1 WHERE Id = @HorseId
        				END

					UPDATE Horse SET MtDNAFlag={2} WHERE OId IN (SELECT OId FROM @TblHorse)
				END
		";

		public const string Get_Horses_For_MtDNA_Lookup = @"
			SELECT
				h.Id, 
				h.OId,
				h.[Name],
				h.Age,
				h.Sex,
				h.Country,
				h.Family,
				CONCAT(hg.Title, '-', ht.[Name]) MtDNATitle,
				hg.Color MtDNAColor,
				hf.[Name] FatherName,
				hm.[Name] MotherName,
				(SELECT COUNT(DISTINCT h1.Id) FROM Horse h1 JOIN Position p ON p.HorseOId=h1.OId WHERE h1.OId IN (SELECT HorseOId FROM Relationship WHERE ParentOId=h.OId) AND dbo.GetBestRaceClass(h1.OId)='G1Wnr') G1Wnrs,
				(SELECT COUNT(DISTINCT h1.Id) FROM Horse h1 JOIN Position p ON p.HorseOId=h1.OId WHERE h1.OId IN (SELECT HorseOId FROM Relationship WHERE ParentOId=h.OId) AND dbo.GetBestRaceClass(h1.OId)='G2Wnr') G2Wnrs,
				(SELECT COUNT(DISTINCT h1.Id) FROM Horse h1 JOIN Position p ON p.HorseOId=h1.OId WHERE h1.OId IN (SELECT HorseOId FROM Relationship WHERE ParentOId=h.OId) AND dbo.GetBestRaceClass(h1.OId)='G3Wnr') G3Wnrs,
				(SELECT COUNT(DISTINCT h1.Id) FROM Horse h1 JOIN Position p ON p.HorseOId=h1.OId WHERE h1.OId IN (SELECT HorseOId FROM Relationship WHERE ParentOId=h.OId) AND dbo.GetBestRaceClass(h1.OId)='SWnr') LRWnrs
			FROM Horse h
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
			LEFT JOIN Horse hf ON hf.OId=rf.ParentOId
			LEFT JOIN Relationship rm ON rm.HorseOId=h.OId AND rm.ParentType='Mother'
			LEFT JOIN Horse hm ON hm.OId=rm.ParentOId
			LEFT JOIN HaploType ht ON ht.Id=h.MtDNA
			LEFT JOIN HaploGroup hg ON hg.Id=ht.GroupId
			WHERE h.isDeleted=0 AND h.Sex='Male' AND hg.Id={0} AND EXISTS (SELECT Place FROM Position WHERE HorseOId IN (SELECT HorseOId FROM Relationship WHERE ParentOId=h.OId)) AND YEAR(CURRENT_TIMESTAMP)-h.Age<=25
		";

		public const string Get_All_Positions = @"
			SELECT p.*, h.Age HorseAge FROM Position p JOIN Horse h ON h.OId=p.HorseOId;
		";

		public const string Save_BPRs = @"
			DECLARE @HorseOId NVARCHAR(50) = '{0}';
			DECLARE @CurrentBPR FLOAT = {1};
			DECLARE @ZCurrentBPR FLOAT = {2};
			DECLARE @HistoricalBPR FLOAT = {3};
			DECLARE @ZHistoricalBPR FLOAT = {4};
			BEGIN TRAN
				UPDATE Coefficient SET CurrentBPR=@CurrentBPR, ZCurrentBPR=@ZCurrentBPR, HistoricalBPR=@HistoricalBPR, ZHistoricalBPR=@ZHistoricalBPR, BPRUpdatedAt=CURRENT_TIMESTAMP WHERE HorseOId=@HorseOId

				IF @@rowcount = 0
				BEGIN
					INSERT INTO Coefficient (HorseOId, CurrentBPR, ZCurrentBPR, HistoricalBPR, ZHistoricalBPR, BPRUpdatedAt) VALUES (@HorseOId, @CurrentBPR, @ZCurrentBPR, @HistoricalBPR, @ZHistoricalBPR, CURRENT_TIMESTAMP)
				END
			COMMIT TRAN
		";

		public const string Get_Stallion_Winners_Count = @"
			SELECT 
				COUNT(h.Id)
			FROM Position p
			LEFT JOIN Horse h ON h.OId=p.HorseOId
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
			WHERE p.Place=1 AND rf.ParentOId='{0}';
		";

        public const string Get_Stallion_Runners_Count = @"
			SELECT 
				COUNT(h.Id)
			FROM Position p
			LEFT JOIN Horse h ON h.OId=p.HorseOId
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
			WHERE rf.ParentOId='{0}';
		";


        public const string Get_Stallion_Avg_Runners_Count = @"
			SELECT
				CAST(SUM(Rnrs) AS FLOAT) / CAST(COUNT(Rnrs) AS FLOAT)
			FROM 
			(
				SELECT
					(SELECT COUNT(*) FROM Position WHERE RaceId=r.Id) Rnrs
				FROM Position p
				LEFT JOIN Race r ON r.Id=p.RaceId
				LEFT JOIN Relationship rf ON rf.HorseOId=p.HorseOId AND rf.ParentType='Father'
				WHERE rf.ParentOId='{0}'
			)
			a
		";

		public const string Get_Expected_Stallion_Wnrs_Value = @"
			SELECT 
				CAST(COUNT(DISTINCT rf.ParentOId) AS FLOAT) / CAST((SELECT COUNT(*) FROM Race) AS FLOAT)
			FROM Position p
			LEFT JOIN Horse h ON h.OId=p.HorseOId
			LEFT JOIN Relationship rf ON rf.HorseOId=h.OId AND rf.ParentType='Father'
			WHERE p.Place=1 AND rf.ParentOId IS NOT NULL
		";

		public const string Delete_AuctionDetails_ByAuctionId = @"
			DELETE FROM AuctionDetail where AuctionId = '{0}'
		";

		public const string Delete_AuctionDetails_ById = @"
			DELETE FROM AuctionDetail where Id = '{0}'
		";

		public const string Get_AuctionDetails_ByAuctionId = @"
			SELECT [Id]
			  ,[AuctionId]
			  ,[LotNumber]
			  ,[Name]
			  ,[Yearling]
			  ,[YOB]
			  ,[Sex]
			  ,[Country]
			  ,[SireId]
			  ,[DamId]
			  ,[mtDNAHapId]
			  ,[CreatedAt]
			  ,[UpdatedAt]
		  FROM [AuctionDetail] where [AuctionId] = '{0}'
		";
	}

    internal class RaceQueryFilterObj
    {
		public string Name { get; set; }
		public int Year { get; set; }
		public string[] Country { get; set; }
		public string[] Distance { get; set; }
		public string[] Surface { get; set; }
		public string[] Type { get; set; }
		public string[] Status { get; set; }

    }
}
