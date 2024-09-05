USE [FishLine]
GO

-- Remember to add "stock nvarchar(50) null" to Age, Animal, AnimalRaised, SpeciesList, SpeciesListRaised and move it up under speciesCode. (If timeout, go to Tools -> Options -> Table Designer and up the timeout)

-- Set all stock ids to NULL on specieslist since they are not used.
UPDATE SpeciesList
SET stockId = NULL
WHERE stockId IS NOT NULL


/* ----- Reset stock table with new stocks ----- */

-- Drop specieslist relationship with L_Stock
ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_L_Stock]
GO

ALTER TABLE [dbo].[R_SpeciesStock] DROP CONSTRAINT [FK_R_SpeciesStock_L_Stock]
GO

ALTER TABLE [dbo].[R_SpeciesStock] DROP CONSTRAINT [FK_R_SpeciesStock_L_Species]
GO

-- Also DROP R_SpeciesStock since it won't be used anymore
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[R_SpeciesStock]') AND type in (N'U'))
DROP TABLE [dbo].[R_SpeciesStock]
GO

TRUNCATE TABLE [L_Stock] 
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[L_Stock]') AND type in (N'U'))
DROP TABLE [dbo].[L_Stock]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[L_Stock](
	[L_stockId] [int] NOT NULL IDENTITY (1,1),
	[stockCode] [nvarchar](50) NOT NULL,
	[description] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_L_Stock] PRIMARY KEY CLUSTERED 
(
	[L_stockId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'aas.27.3031', N'Noble crayfish in Baltic Subdivisions 30 and 31 - test stock')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'agn.27.nea', N'Angel shark (Squatina squatina) in Subareas 1-10  12 and 14 (the Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'alf.27.nea', N'Alfonsinos (Beryx spp.) in Subareas 1-10  12 and 14 (the Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ane.27.8', N'Anchovy (Engraulis encrasicolus) in Subarea 8 (Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ane.27.9a', N'Anchovy (Engraulis encrasicolus) in Division 9.a (Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'anf.27.1-2', N'Anglerfish (Lophius budegassa  Lophius piscatorius) in Subareas 1 and 2 (Northeast Arctic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'anf.27.3a46', N'Anglerfish (Lophius budegassa  Lophius piscatorius) in Subareas 4 and 6  and Division 3.a (North Sea  Rockall and West of Scotland  Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ank.27.78abd', N'Black-bellied anglerfish (Lophius budegassa) in Subarea 7 and divisions 8.a-b and 8.d (Celtic Seas  Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ank.27.8c9a', N'Black-bellied anglerfish (Lophius budegassa) in Divisions 8.c and 9.a (Cantabrian Sea  Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'aru.27.123a4', N'Greater silver smelt (Argentina silus) in Subareas 1  2  and 4  and Division 3.a (Northeast Arctic  North Sea  Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'aru.27.5a14', N'Greater silver smelt (Argentina silus) in Subarea 14 and Division 5.a (East Greenland and Iceland grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'aru.27.5b6a', N'Greater silver smelt (Argentina silus) in Divisions 5.b and 6.a (Faroes grounds and west of Scotland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'aru.27.6b7-1012', N'Greater silver smelt (Argentina silus) in Subareas 7-10 and 12  and in Division 6.b (other areas)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'bli.27.5a14', N'Blue ling (Molva dypterygia) in Subarea 14 and Division 5.a (East Greenland and Iceland grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'bli.27.5b67', N'Blue ling (Molva dypterygia) in Subareas 6-7 and Division 5.b (Celtic Seas  English Channel  and Faroes grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'bli.27.nea', N'Blue ling (Molva dypterygia) in Subareas 1  2  8  9  and 12  and Divisions 3.a and 4.a (other areas)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'bll.27.22-32', N'Brill (Scophthalmus rhombus) in subdivisions 22-32 (Baltic Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'bll.27.3a47de', N'Brill (Scophthalmus rhombus) in Subarea 4 and Divisions 3.a and 7.d-e (North Sea  Skagerrak and Kattegat  English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'boc.27.6-8', N'Boarfish (Capros aper) in Subareas 6-8 (Celtic Seas  English Channel  and Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'bsf.27.nea', N'Black scabbardfish (Aphanopus carbo) in Subareas 1  2  4-8  10  and 14  and in Divisions 3.a  9.a  and 12.b (Northeast Atlantic and Arctic Ocean)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'bsk.27.nea', N'Basking shark (Cetorhinus maximus) in Subareas 1-10  12 and 14 (the Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'bss.27.4bc7ad-h', N'Seabass (Dicentrarchus labrax) in Divisions 4.b-c  7.a  and 7.d-h (central and southern North Sea  Irish Sea  English Channel  Bristol Channel  and Celtic Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'bss.27.6a7bj', N'Seabass (Dicentrarchus labrax) in Divisions 6.a  7.b  and 7.j (West of Scotland  West of Ireland  eastern part of southwest of Ireland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'bss.27.8ab', N'Seabass (Dicentrarchus labrax) in Divisions 8.a-b (northern and central Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'bss.27.8c9a', N'Seabass (Dicentrarchus labrax) in Divisions 8.c and 9.a (southern Bay of Biscay and Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'bwp.27.2729-32', N'Baltic flounder (Platichthys solemdali) in Subdivisions 27 and 29-32 (northern central and northern Baltic Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'bwq.27.2425', N'Flounder (Platichthys spp) in subdivisions 24 and 25 (west of Bornholm and southwestern central Baltic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'bwq.27.2628', N'Flounder (Platichthys spp) in Subdivisions 26 and 28 (east of Gotland and Gulf of Gdansk)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'caa.27.5a', N'Atlantic wolffish (Anarhichas lupus) in Division 5.a (Iceland grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'caa.27.3a47de', N'Atlantic wolffish (Anarhichas lupus) in in Subarea 4 and divisions 3.a  7d-e (North Sea  Skagerrak and Kattegat  English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cap.27.1-2', N'Capelin (Mallotus villosus) in Subareas 1 and 2 (Northeast Arctic)  excluding Division 2.a west of 5�W (Barents Sea capelin)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cap.27.2a514', N'Capelin (Mallotus villosus) in Subareas 5 and 14 and Division 2.a west of 5�W (Iceland and Faroes grounds  East Greenland  Jan Mayen area)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.21.1', N'Cod (Gadus morhua) in NAFO Subarea 1  inshore (West Greenland cod)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.21.1a-e', N'Cod (Gadus morhua) in NAFO Divisions 1.A-E  offshore (West Greenland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.2127.1f14', N'Cod (Gadus morhua) in ICES Subarea 14 and NAFO Division 1.F (East Greenland  South Greenland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.27.1-2', N'Cod (Gadus morhua) in Subareas 1 and 2 (Northeast Arctic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.27.1-2coast', N'Cod (Gadus morhua) in Subareas 1 and 2 (Norwegian coastal waters cod)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.27.21', N'Cod (Gadus morhua) in Subdivision 21 (Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.27.22-24', N'Cod (Gadus morhua) in Subdivisions 22-24  western Baltic stock (western Baltic Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.27.24-32', N'Cod (Gadus morhua) in Subdivisions 24-32  eastern Baltic stock (eastern Baltic Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.27.47d20', N'Cod (Gadus morhua) in Subarea 4  Division 7.d  and Subdivision 20 (North Sea  eastern English Channel  Skagerrak)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.27.5a', N'Cod (Gadus morhua) in Division 5.a (Iceland grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.27.5b1', N'Cod (Gadus morhua) in Subdivision 5.b.1 (Faroe Plateau)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.27.5b2', N'Cod (Gadus morhua) in Subdivision 5.b.2 (Faroe Bank)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.27.6a', N'Cod (Gadus morhua) in Division 6.a (West of Scotland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.27.6b', N'Cod (Gadus morhua) in Division 6.b (Rockall)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.27.7a', N'Cod (Gadus morhua) in Division 7.a (Irish Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cod.27.7e-k', N'Cod (Gadus morhua) in Divisions 7.e-k (eastern English Channel and southern Celtic Seas)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'coe.27.3a47de', N'European conger (Conger conger) in Subarea 4 and divisions 3.a  7d-e (North Sea  Skagerrak and Kattegat  English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ctc.27.nea', N'Common cuttlefish (Sepia officinalis) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ctl.27.nea', N'Cuttlefishes (Sepiida) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cvt.27.nea', N'Giant African cuttlefish (Sepia hierredda) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'cyo.27.nea', N'Portuguese dogfish (Centroscymnus coelolepis  Centrophorus squamosus) in Subareas 1-10  12 and 14 (the Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'dab.27.22-32', N'Dab (Limanda limanda) in Subdivisions 22-32 (Baltic Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'dab.27.3a4', N'Dab (Limanda limanda) in Subarea 4 and Division 3.a (North Sea  Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'dgs.27.nea', N'Spurdog (Squalus acanthias) in Subareas 1-10  12 and 14 (the Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'edt.27.nea', N'Musky octopus (Eledone moschata) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'eje.27.nea', N'Elegant cuttlefish (Sepia elegans) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ele.2737.nea', N'European eel (Anguilla anguilla) throughout its natural range')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'eoi.27.nea', N'Horned octopus (Eledone cirrhosa) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'fle.27.2223', N'Flounder (Platichthys flesus) in Subdivisions 22 and 23 (Belt Seas and the Sound)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'fle.27.3a4', N'Flounder (Platichthys flesus) in Subarea 4 and Division 3.a (North Sea  Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'gag.27.nea', N'Tope (Galeorhinus galeus) in Subareas 1-10  12 and 14 (the Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'gfb.27.nea', N'Greater forkbeard (Phycis blennoides) in Subareas 1-10  12 and 14 (the Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ghl.27.1-2', N'Greenland halibut (Reinhardtius hippoglossoides) in Subareas 1 and 2 (Northeast Arctic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ghl.27.561214', N'Greenland halibut (Reinhardtius hippoglossoides) in Subareas 5  6  12  and 14 (Iceland and Faroes grounds  West of Scotland  North of Azores  East of Greenland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'gti.27.nea', N'Boreoatlantic gonate squid (Gonatus fabricii) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'gug.27.3a47d', N'Grey gurnard (Eutrigla gurnardus) in Subarea 4 and Divisions 7.d and 3.a (North Sea  eastern English Channel  Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'guq.27.nea', N'Leafscale gulper shark (Centrophorus squamosus) in Subareas 1-10  12 and 14 (the Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'gur.27.3-8', N'Red gurnard (Chelidonichthys cuculus) in Subareas 3-8 (Northeast Atlantic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'had.27.1-2', N'Haddock (Melanogrammus aeglefinus) in Subareas 1 and 2 (Northeast Arctic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'had.27.46a20', N'Haddock (Melanogrammus aeglefinus) in Subarea 4  Division 6.a  and Subdivision 20 (North Sea  West of Scotland  Skagerrak)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'had.27.5a', N'Haddock (Melanogrammus aeglefinus) in Division 5.a (Iceland grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'had.27.5b', N'Haddock (Melanogrammus aeglefinus) in Division 5.b (Faroes grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'had.27.6b', N'Haddock (Melanogrammus aeglefinus) in Division 6.b (Rockall)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'had.27.7a', N'Haddock (Melanogrammus aeglefinus) in Division7.a (Irish Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'had.27.7b-k', N'Haddock (Melanogrammus aeglefinus) in Divisions 7.b-k (southern Celtic Seas and English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'hal.27.3a47de', N'Atlantic halibut (Hippoglossus hippoglossus) in Subarea 4 and divisions 3.a  7d-e (North Sea  Skagerrak and Kattegat  English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'her.27.1-24a514a', N'Herring (Clupea harengus) in Subareas 1  2  5 and Divisions 4.a and 14.a  Norwegian spring-spawning herring (the Northeast Atlantic and Arctic Ocean)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'her.27.20-24', N'Herring (Clupea harengus) in Subdivisions 20-24  spring spawners (Skagerrak  Kattegat  and western Baltic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'her.27.25-2932', N'Herring (Clupea harengus) in Subdivisions 25-29 and 32  excluding the Gulf of Riga (central Baltic Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'her.27.28', N'Herring (Clupea harengus) in Subdivision 28.1 (Gulf of Riga)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'her.27.3031', N'Herring (Clupea harengus) in Subdivisions 30 and 31 (Gulf of Bothnia)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'her.27.3a47d', N'Herring (Clupea harengus) in Subarea 4 and Divisions 3.a and 7.d  autumn spawners (North Sea  Skagerrak and Kattegat  eastern English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'her.27.5a', N'Herring (Clupea harengus) in Division 5.a  summer-spawning herring (Iceland grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'her.27.6a7bc', N'Herring (Clupea harengus) in Divisions 6.a and 7.b-c (West of Scotland  West of Ireland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'her.27.irls', N'Herring (Clupea harengus) in Divisions 7.a South of 52�30?N  7.g-k (Irish Sea  Celtic Sea  and southwest of Ireland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'her.27.nirs', N'Herring (Clupea harengus) in Division 7.a North of 52�30?N (Irish Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'hke.27.3a46-8abd', N'Hake (Merluccius merluccius) in Subareas 4  6  and 7  and Divisions 3.a  8.a-b  and 8.d  Northern stock (Greater North Sea  Celtic Seas  and the northern Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'hke.27.8c9a', N'Hake (Merluccius merluccius) in Divisions 8.c and 9.a  Southern stock (Cantabrian Sea and Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'hom.27.2a4a5b6a7a-ce-k8', N'Horse mackerel (Trachurus trachurus) in Subarea 8 and Divisions 2.a  4.a  5.b  6.a  7.a-c e-k (the Northeast Atlantic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'hom.27.3a4bc7d', N'Horse mackerel (Trachurus trachurus) in Divisions 3.a  4.b-c  and 7.d (Skagerrak and Kattegat  southern and central North Sea  eastern English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'hom.27.9a', N'Horse mackerel (Trachurus trachurus) in Division 9.a (Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'iar.27.nea', N'Pink cuttlefish (Sepia orbignyana) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ill.27.nea', N'Shortfin squids (Illex) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'jaa.27.10a2', N'Blue jack mackerel (Trachurus picturatus) in Subdivision 10.a.2 (Azores grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ldb.27.7b-k8abd', N'Four-spot megrim (Lepidorhombus boscii) in Divisions 7.b-k  8.a-b  and 8.d (west and southwest of Ireland  Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ldb.27.8c9a', N'Four-spot megrim (Lepidorhombus boscii) in Divisions 8.c and 9.a (southern Bay of Biscay and Atlantic Iberian waters East)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'lem.27.3a47d', N'Lemon sole (Microstomus kitt) in Subarea 4 and Divisions 3.a and 7.d (North Sea  Skagerrak and Kattegat  eastern English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'lez.27.4a6a', N'Megrim (Lepidorhombus spp.) in Divisions 4.a and 6.a (northern North Sea  West of Scotland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'lez.27.6b', N'Megrim (Lepidorhombus spp.) in Division 6.b (Rockall)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'lin.27.1-2', N'Ling (Molva molva) in Subareas 1 and 2 (Northeast Arctic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'lin.27.3a4a6-91214', N'Ling (Molva molva) in Subareas 6-9  12  and 14  and Divisions 3.a and 4.a (Northeast Atlantic and Arctic Ocean)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'lin.27.5a', N'Ling (Molva molva) in Division 5.a (Iceland grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'lin.27.5b', N'Ling (Molva molva) in Division 5.b (Faroes grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'mac.27.nea', N'Mackerel (Scomber scombrus) in Subareas 1-8 and 14  and Division 9.a (the Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'meg.27.7b-k8abd', N'Megrim (Lepidorhombus whiffiagonis) in Divisions 7.b-k  8.a-b  and 8.d (west and southwest of Ireland  Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'meg.27.8c9a', N'Megrim (Lepidorhombus whiffiagonis) in Divisions 8.c and 9.a (Cantabrian Sea and Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'mon.27.78ab', N'White anglerfish (Lophius piscatorius) in Divisions 7.b-k  8.a-b  and 8.d (southern Celtic Seas  Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'mon.27.78abd', N'White anglerfish (Lophius piscatorius) in Subarea 7 and divisions 8.a-b and 8.d (Celtic Seas  Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'mon.27.8c9a', N'White anglerfish (Lophius piscatorius) in Divisions 8.c and 9.a (Cantabrian Sea and Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'mur.27.3a47d', N'Striped red mullet (Mullus surmuletus) in Subarea 4 and Divisions 7.d and 3.a (North Sea  eastern English Channel  Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'mur.27.67a-ce-k89a', N'Striped red mullet (Mullus surmuletus) in Subareas 6 and 8  and Divisions 7.a-c  7.e-k  and 9.a (North Sea  Bay of Biscay  southern Celtic Seas  and Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.27.4outFU', N'Norway lobster (Nephrops norvegicus) in Subarea 4  outside the Functional Units (North Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.27.6aoutFU', N'Norway lobster (Nephrops norvegicus) in Division 6.a  outside the Functional Units (West of Scotland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.27.7outFU', N'Norway lobster (Nephrops norvegicus) in Subarea 7  outside the Functional Units (southern Celtic Seas  southwest of Ireland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.10', N'Norway lobster (Nephrops norvegicus) in Division 4.a  Functional Unit 10 (northern North Sea  Noup)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.11', N'Norway lobster (Nephrops norvegicus) in Division 6.a  Functional Unit 11 (West of Scotland  North Minch)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.12', N'Norway lobster (Nephrops norvegicus) in Division 6.a  Functional Unit 12 (West of Scotland  South Minch)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.13', N'Norway lobster (Nephrops norvegicus) in Division 6.a  Functional Unit 13 (West of Scotland  the Firth of Clyde and Sound of Jura)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.14', N'Norway lobster (Nephrops norvegicus) in Division 7.a  Functional Unit 14 (Irish Sea  East)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.15', N'Norway lobster (Nephrops norvegicus) in Division 7.a  Functional Unit 15 (Irish Sea  West)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.16', N'Norway lobster (Nephrops norvegicus) in Divisions 7.b-c and 7.j-k  Functional Unit 16 (west and southwest of Ireland  Porcupine Bank)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.17', N'Norway lobster (Nephrops norvegicus) in Division 7.b  Functional Unit 17 (west of Ireland  Aran grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.19', N'Norway lobster (Nephrops norvegicus) in divisions 7.a  7.g  and 7.j  Functional Unit 19 (Irish Sea  Celtic Sea  eastern part of southwest of Ireland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.2021', N'Norway lobster (Nephrops norvegicus) in Divisions 7.g and 7.h  Functional Units 20 and 21 (Celtic Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.22', N'Norway lobster (Nephrops norvegicus) in Divisions 7.f and 7.g  Functional Unit 22 (Celtic Sea  Bristol Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.2324', N'Norway lobster (Nephrops norvegicus) in Divisions 8.a and 8.b  Functional Units 23-24 (northern and central Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.25', N'Norway lobster (Nephrops norvegicus) in Division 8.c  Functional Unit 25 (southern Bay of Biscay and northern Galicia)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.2627', N'Norway lobster (Nephrops norvegicus) in Division 9.a  Functional Units 26-27 (Atlantic Iberian waters East  western Galicia  and northern Portugal)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.2829', N'Norway lobster (Nephrops norvegicus) in Division 9.a  Functional Units 28-29 (Atlantic Iberian waters East and southwestern and southern Portugal)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.30', N'Norway lobster (Nephrops norvegicus) in Division 9.a  Functional Unit 30 (Atlantic Iberian waters East and Gulf of Cadiz)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.31', N'Norway lobster (Nephrops norvegicus) in Division 8.c  Functional Unit 31 (southern Bay of Biscay and Cantabrian Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.32', N'Norway lobster (Nephrops norvegicus) in Division 4.a  Functional Unit 32 (northern North Sea  Norway Deep)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.33', N'Norway lobster (Nephrops norvegicus) in Division 4.b  Functional Unit 33 (central North Sea  Horn?s Reef)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.34', N'Norway lobster (Nephrops norvegicus) in Division 4.b  Functional Unit 34 (central North Sea  Devil?s Hole)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.3-4', N'Norway lobster (Nephrops norvegicus) in Division 3.a  Functional Units 3 and 4 (Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.5', N'Norway lobster (Nephrops norvegicus) in Divisions 4.b and 4.c  Functional Unit 5 (central and southern North Sea  Botney Cut-Silver Pit)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.6', N'Norway lobster (Nephrops norvegicus) in Division 4.b  Functional Unit 6 (central North Sea  Farn Deeps)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.7', N'Norway lobster (Nephrops norvegicus) in Division 4.a  Functional Unit 7 (northern North Sea  Fladen Ground)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.8', N'Norway lobster (Nephrops norvegicus) in Division 4.b  Functional Unit 8 (central North Sea  Firth of Forth)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nep.fu.9', N'Norway lobster (Nephrops norvegicus) in Division 4.a  Functional Unit 9 (central North Sea  Moray Firth)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nop.27.3a4', N'Norway pout (Trisopterus esmarkii) in Subarea 4 and Division 3.a (North Sea  Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'nop.27.6a', N'Norway pout (Trisopterus esmarkii) in Division 6.a')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'occ.27.nea', N'Common octopus (Octopus vulgaris) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ocm.27.nea', N'Horned and musky octopuses (Eledone) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'oct.27.nea', N'Octopods (Octopodidae) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ocz.27.nea', N'Octopuses (Octopus) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'omz.27.nea', N'Ommastrephidae squids (Ommastrephidae) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ory.27.nea', N'Orange roughy (Hoplostethus atlanticus) in Subareas 1-10  12 and 14 (the Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'oul.27.nea', N'European common squid (Alloteuthis subulata) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'oum.27.nea', N'Midsize squid (Alloteuthis media) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ouw.27.nea', N'Alloteuthis squids (Alloteuthis) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'pil.27.7', N'Sardine (Sardina pilchardus) in Subarea 7 (Southern Celtic Seas  English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'pil.27.8abd', N'Sardine (Sardina pilchardus) in Divisions 8.a-b and 8.d (Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'pil.27.8c9a', N'Sardine (Sardina pilchardus) in Divisions 8.c and 9.a (Cantabrian Sea and Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ple.27.5a', N'Plaice (Pleuronectes platessa) in Division 5.a (Iceland grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ple.27.21-23', N'Plaice (Pleuronectes platessa) in Subdivisions 21-23 (Kattegat  Belt Seas  and the Sound)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ple.27.24-32', N'Plaice (Pleuronectes platessa) in Subdivisions 24-32 (Baltic Sea  excluding the Sound and Belt Seas)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ple.27.420', N'Plaice (Pleuronectes platessa) in Subarea 4 (North Sea) and Subdivision 20 (Skagerrak)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ple.27.7a', N'Plaice (Pleuronectes platessa) in Division 7.a (Irish Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ple.27.7bc', N'Plaice (Pleuronectes platessa) in Divisions 7.b-c (West of Ireland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ple.27.7d', N'Plaice (Pleuronectes platessa) in Division 7.d (eastern English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ple.27.7e', N'Plaice (Pleuronectes platessa) in Division 7.e (western English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ple.27.7fg', N'Plaice (Pleuronectes platessa) in Divisions 7.f and 7.g (Bristol Channel  Celtic Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ple.27.7h-k', N'Plaice (Pleuronectes platessa) in Divisions 7.h-k (Celtic Sea South  Southwest of Ireland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ple.27.89a', N'Plaice (Pleuronectes platessa) in Subarea 8 and Division 9.a (Bay of Biscay and Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'pok.27.1-2', N'Saithe (Pollachius virens) in Subareas 1 and 2 (Northeast Arctic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'pok.27.3a46', N'Saithe (Pollachius virens) in Subareas 4  6 and Division 3.a (North Sea  Rockall and West of Scotland  Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'pok.27.5a', N'Saithe (Pollachius virens) in Division 5.a (Iceland grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'pok.27.5b', N'Saithe (Pollachius virens) in Division 5.b (Faroes grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'pok.27.7-10', N'Saithe (Pollachius virens) in subareas 7-10 (Southern Celtic Sea and the English Channel  Bay of Biscay  Atlantic Iberian waters and the Azores grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'pol.27.3a4', N'Pollack (Pollachius pollachius) in Subarea 4 and Division 3.a (North Sea  Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'pol.27.67', N'Pollack (Pollachius pollachius) in Subareas 6-7 (Celtic Seas and the English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'pol.27.89a', N'Pollack (Pollachius pollachius) in Subarea 8 and Division 9.a (Bay of Biscay and Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'por.27.nea', N'Porbeagle (Lamna nasus) in Subareas 1-10  12 and 14 (the Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'pra.27.1-2', N'Northern shrimp (Pandalus borealis) in Subareas 1 and 2 (Northeast Arctic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'pra.27.3a4a', N'Northern shrimp (Pandalus borealis) in divisions 3.a and 4.a East (Skagerrak and Kattegat and northern North Sea in the Norwegian Deep)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'pra.27.4a', N'Northern shrimp (Pandalus borealis) in Division 4.a West (northern North Sea  Fladen Ground)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'raj.27.1012', N'Rays and skates (Rajidae) (mainly thornback ray (Raja clavata)) in Subareas 10 and 12 (Azores grounds and north of Azores)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'raj.27.3a47d', N'Rays and skates (Rajidae) in Subarea 4 and Divisions 3.a and 7.d (North Sea  Skagerrak  Kattegat  and eastern English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'raj.27.67a-ce-h', N'Rays and skates (Rajidae) in Subarea 6 and Divisions 7.a-c and 7.e-h (Rockall and West of Scotland  southern Celtic Seas  western English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'raj.27.89a', N'Rays and skates (Rajidae) in Subarea 8 and Division 9.a (Bay of Biscay and Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'reb.2127.dp', N'Beaked redfish (Sebastes mentella) in ICES Subareas 5  12  and 14 (Iceland and Faroes grounds  North of Azores  East of Greenland) and NAFO Subareas 1 and 2 (deep pelagic stock > 500 m)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'reb.2127.sp', N'Beaked redfish (Sebastes mentella) in ICES Subareas 5  12  and 14 (Iceland and Faroes grounds  North of Azores  East of Greenland) and NAFO Subareas 1 and 2 (shallow pelagic stock < 500 m)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'reb.27.1-2', N'Beaked redfish (Sebastes mentella) in Subareas 1 and 2 (Northeast Arctic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'reb.27.14b', N'Beaked redfish (Sebastes mentella) in Division 14.b  demersal (Southeast Greenland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'reb.27.5a14', N'Beaked redfish (Sebastes mentella) in Subarea 14 and Division 5.a  Icelandic slope stock (East of Greenland  Iceland grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'reg.27.1-2', N'Golden redfish (Sebastes norvegicus) in Subareas 1 and 2 (Northeast Arctic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'reg.27.561214', N'Golden redfish (Sebastes norvegicus) in Subareas 5  6  12  and 14 (Iceland and Faroes grounds  West of Scotland  North of Azores  East of Greenland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rhg.27.nea', N'Roughhead grenadier (Macrourus berglax) in Subareas 5-8  10  12 and 14 (Northeast Atlantic and Arctic Ocean)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rja.27.nea', N'White skate (Rostroraja alba) in Subareas 1-10  12 and 14 (the Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjb.27.3a4', N'Common skate complex (Blue skate (Dipturus batis) and flapper skate (Dipturus intermedius) in Subarea 4 and Division 3.a (North Sea  Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjb.27.67a-ce-k', N'Common skate complex (Blue skate (Dipturus batis) and flapper skate (Dipturus intermedius) in Subarea 6 and Divisions 7.a-c and 7.e-k (Celtic Seas and western English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjb.27.89a', N'Common skate complex (Blue skate (Dipturus batis) and flapper skate (Dipturus intermedius) in Subarea 8 and Division 9.a (Bay of Biscay and Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjc.27.3a47d', N'Thornback ray (Raja clavata) in Subarea 4 and Divisions 3.a and 7.d (North Sea  Skagerrak  Kattegat  and eastern English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjc.27.6', N'Thornback ray (Raja clavata) in Subarea 6 (West of Scotland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjc.27.7afg', N'Thornback ray (Raja clavata) in Divisions 7.a and 7.f-g (Irish Sea  Bristol Channel  Celtic Sea North)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjc.27.7e', N'Thornback ray (Raja clavata) in Division 7.e (western English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjc.27.8', N'Thornback ray (Raja clavata) in Subarea 8 (Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjc.27.9a', N'Thornback ray (Raja clavata) in Division 9.a (Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rje.27.7de', N'Small-eyed ray (Raja microocellata) in Divisions 7.d and 7.e (English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rje.27.7fg', N'Small-eyed ray (Raja microocellata) in Divisions 7.f and 7.g (Bristol Channel  Celtic Sea North)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjf.27.67', N'Shagreen ray (Leucoraja fullonica) in Subareas 6-7 (West of Scotland  southern Celtic Seas  English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjh.27.4a6', N'Blonde ray (Raja brachyura) in Subarea 6 and Division 4.a (North Sea and West of Scotland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjh.27.4c7d', N'Blonde ray (Raja brachyura) in Divisions 4.c and 7.d (southern North Sea and eastern English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjh.27.7afg', N'Blonde ray (Raja brachyura) in Divisions 7.a and 7.f-g (Irish Sea  Bristol Channel  Celtic Sea North)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjh.27.7e', N'Blonde ray (Raja brachyura) in Division 7.e (western English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjh.27.9a', N'Blonde ray (Raja brachyura) in Division 9.a (Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rji.27.67', N'Sandy ray (Leucoraja circularis) in Subareas 6-7 (West of Scotland  southern Celtic Seas  English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjm.27.3a47d', N'Spotted ray (Raja montagui) in Subarea 4 and Divisions 3.a and 7.d (North Sea  Skagerrak  Kattegat  and eastern English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjm.27.67bj', N'Spotted ray (Raja montagui) in Subarea 6 and Divisions 7.b and 7.j (West of Scotland  west and southwest of Ireland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjm.27.7ae-h', N'Spotted ray (Raja montagui) in Divisions 7.a and 7.e-h (southern Celtic Seas and western English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjm.27.8', N'Spotted ray (Raja montagui) in Subarea 8 (Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjm.27.9a', N'Spotted ray (Raja montagui) in Division 9.a (Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjn.27.3a4', N'Cuckoo ray (Leucoraja naevus) in Subarea 4 and Division 3.a (North Sea  Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjn.27.678abd', N'Cuckoo ray (Leucoraja naevus) in subareas 6-7 and divisions 8.a-b and 8.d (West of Scotland  southern Celtic Seas  and western English Channel  Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjn.27.8c', N'Cuckoo ray (Leucoraja naevus) in Division 8.c (Cantabrian Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjn.27.9a', N'Cuckoo ray (Leucoraja naevus) in Division 9.a (Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rjr.27.23a4', N'Starry ray (Amblyraja radiata) in subareas 2 and 4  and Division 3.a (Norwegian Sea  North Sea  Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rju.27.7bj', N'Undulate ray (Raja undulata) in divisions 7.b and 7.j (west and southwest of Ireland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rju.27.7de', N'Undulate ray (Raja undulata) in divisions 7.d and 7.e (English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rju.27.8ab', N'Undulate ray (Raja undulata) in divisions 8.a-b (northern and central Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rju.27.8c', N'Undulate ray (Raja undulata) in Division 8.c (Cantabrian Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rju.27.9a', N'Undulate ray (Raja undulata) in Division 9.a (Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rng.27.1245a8914ab', N'Roundnose grenadier (Coryphaenoides rupestris) in Subareas 1  2  4  8  and 9  Division 14.a  and Subdivisions 14.b.2 and 5.a.2 (Northeast Atlantic and Arctic Ocean)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rng.27.3a', N'Roundnose grenadier (Coryphaenoides rupestris) in Division 3.a (Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rng.27.5a10b12ac14b', N'Roundnose grenadier (Coryphaenoides rupestris) in Divisions 10.b and 12.c  and in Subdivisions 12.a.1  14.b.1  and 5.a.1 (Oceanic Northeast Atlantic and northern Reykjanes Ridge)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'rng.27.5b6712b', N'Roundnose grenadier (Coryphaenoides rupestris) in Subareas 6-7  and in Divisions 5.b and 12.b (Celtic Seas and the English Channel  Faroes grounds  and western Hatton Bank)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'roa.27.nea', N'Stout bobtail squid (Rossia macrosoma) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sal.27.22-31', N'Salmon (Salmo salar) in Baltic Subdivisions 22-31 (Baltic Sea  excluding the Gulf of Finland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sal.27.32', N'Salmon (Salmo salar) in Baltic Subdivision 32 (Gulf of Finland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sal.nac.all', N'Salmon (Salmo salar) from North America')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sal.neac.all', N'Salmon (Salmo salar) in Northeast Atlantic and Arctic Ocean  excluding Subareas 3 and 14')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sal.wgc.all', N'Salmon (Salmo salar) in Subarea 14 and NAFO Subarea 1 (east and west of Greenland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'san.27.6a', N'Sandeel (Ammodytes spp.) in Division 6.a (West of Scotland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'san.sa.1r', N'Sandeel (Ammodytes spp.) in Divisions 4.b and 4.c  Sandeel Area 1r (central and southern North Sea  Dogger Bank)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'san.sa.2r', N'Sandeel (Ammodytes spp.) in Divisions 4.b and 4.c  and Subdivision 20  Sandeel Area 2r (Skagerrak  central and southern North Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'san.sa.3r', N'Sandeel (Ammodytes spp.) in Divisions 4.a and 4.b  and Subdivision 20  Sandeel Area 3r (Skagerrak  northern and central North Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'san.sa.4', N'Sandeel (Ammodytes spp.) in Divisions 4.a and 4.b  Sandeel Area 4 (northern and central North Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'san.sa.5r', N'Sandeel (Ammodytes spp.) in Division 4.a  Sandeel Area 5r (northern North Sea  Viking and Bergen banks)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'san.sa.6', N'Sandeel (Ammodytes spp.) in Subdivisions 20-22  Sandeel Area 6 (Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'san.sa.7r', N'Sandeel (Ammodytes spp.) in Division 4.a  Sandeel Area 7r (northern North Sea  Shetland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sbr.27.10', N'Blackspot seabream (Pagellus bogaraveo) in Subarea 10 (Azores grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sbr.27.6-8', N'Blackspot seabream (Pagellus bogaraveo) in Subareas 6-8 (Celtic Seas  the English Channel  and Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sbr.27.9', N'Blackspot seabream (Pagellus bogaraveo) in Subarea 9 (Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sce.sca.ne', N'Great Atlantic scallop (Pecten maximus) in the Scallop area NE (Northeast of Scotland)  test stock')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sce.sca.nw', N'Great Atlantic scallop (Pecten maximus) in the Scallop area NW (Northwest of Scotland)  test stock')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sck.27.nea', N'Kitefin shark (Dalatias licha) in Subareas 1-10  12 and 14 (the Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sdv.27.nea', N'Smooth-hound (Mustelus spp.) in Subareas 1-10  12 and 14 (the Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sho.27.67', N'Black-mouth dogfish (Galeus melastomus) in Subareas 6 and 7 (West of Scotland  southern Celtic Seas  and English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sho.27.89a', N'Black-mouth dogfish (Galeus melastomus) in Subarea 8 and Division 9.a (Bay of Biscay and Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sol.27.20-24', N'Sole (Solea solea) in Subdivisions 20-24 (Skagerrak and Kattegat  western Baltic Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sol.27.4', N'Sole (Solea solea) in Subarea 4 (North Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sol.27.7a', N'Sole (Solea solea) in Division 7.a (Irish Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sol.27.7bc', N'Sole (Solea solea) in Divisions 7.b and 7.c (West of Ireland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sol.27.7d', N'Sole (Solea solea) in Division 7.d (eastern English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sol.27.7e', N'Sole (Solea solea) in Division 7.e (western English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sol.27.7fg', N'Sole (Solea solea) in Divisions 7.f and 7.g (Bristol Channel  Celtic Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sol.27.7h-k', N'Sole (Solea solea) in Divisions 7.h-k (Celtic Sea South  southwest of Ireland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sol.27.8ab', N'Sole (Solea solea) in Divisions 8.a-b (northern and central Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sol.27.8c9a', N'Sole (Solea solea) in Divisions 8.c and 9.a (Cantabrian Sea and Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'spr.27.22-32', N'Sprat (Sprattus sprattus) in subdivisions 22-32 (Baltic Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'spr.27.3a4', N'Sprat (Sprattus sprattus) in Division 3.a and Subarea 4 (Skagerrak  Kattegat and North Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'spr.27.67a-cf-k', N'Sprat (Sprattus sprattus) in Subarea 6 and Divisions 7.a-c and 7.f-k (West of Scotland  southern Celtic Seas)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'spr.27.7de', N'Sprat (Sprattus sprattus) in Divisions 7.d and 7.e (English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sqc.27.nea', N'Common squids (Loligo) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sqe.27.nea', N'European flying squid (Todarodes sagittatus) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sqf.27.nea', N'Veined squid (Loligo forbesii) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sqm.27.nea', N'Broadtail shortfin squid (Illex coindetii) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sqr.27.nea', N'European squid (Loligo vulgaris) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'squ.27.nea', N'Squids nei (Loliginidae, Ommastrephidae) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'sqz.27.nea', N'Inshore squids (Loliginidae) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'syc.27.3a47d', N'Lesser-spotted dogfish (Scyliorhinus canicula) in Subarea 4 and in Divisions 3.a and 7.d (North Sea  Skagerrak and Kattegat  eastern English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'syc.27.67a-ce-j', N'Lesser-spotted dogfish (Scyliorhinus canicula) in Subarea 6 and Divisions 7.a-c and 7.e-j (West of Scotland  Irish Sea  southern Celtic Seas)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'syc.27.8abd', N'Lesser-spotted dogfish (Scyliorhinus canicula) in Divisions 8.a-b and 8.d (Bay of Biscay)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'syc.27.8c9a', N'Lesser-spotted dogfish (Scyliorhinus canicula) in Divisions 8.c and 9.a (Cantabrian Sea and Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'syt.27.67', N'Greater-spotted dogfish (Scyliorhinus stellaris) in Subareas 6 and 7 (West of Scotland  southern Celtic Sea  and the English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'tdq.27.nea', N'Lesser flying squid (Todaropsis eblanae) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'thr.27.nea', N'Thresher sharks (Alopias spp.) in Subareas 10  12  Divisions 7.c-k  8.d-e  and Subdivisions 5.b.1  9.b.1  14.b.1 (Northeast Atlantic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'trs.27.22-32', N'Sea trout (Salmo trutta) in Subdivisions 22-32 (Baltic Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'tsu.27.nea', N'Roughsnout grenadier (Trachyrincus scabrus) in Subareas 1-2  4-8  10  12  14 and Division 3.a (Northeast Atlantic and Arctic Ocean)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'tur.27.22-32', N'Turbot (Scophthalmus maximus) in Subdivisions 22-32 (Baltic Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'tur.27.3a', N'Turbot (Scophthalmus maximus) in Division 3.a (Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'tur.27.4', N'Turbot (Scophthalmus maximus) in Subarea 4 (North Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'usk.27.1-2', N'Tusk (Brosme brosme) in Subareas 1 and 2 (Northeast Arctic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'usk.27.12ac', N'Tusk (Brosme brosme) in Subarea 12  excluding Division 12.b (southern Mid-Atlantic Ridge)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'usk.27.3a45b6a7-912b', N'Tusk (Brosme brosme) in Subareas 4 and 7-9  and in Divisions 3.a  5.b  6.a  and 12.b (Northeast Atlantic)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'usk.27.5a14', N'Tusk (Brosme brosme) in Subarea 14 and Division 5.a (East Greenland  and Iceland grounds)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'usk.27.6b', N'Tusk (Brosme brosme) in Division 6.b (Rockall)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'whb.27.1-91214', N'Blue whiting (Micromesistius poutassou) in Subareas 1-9  12  and 14 (Northeast Atlantic and adjacent waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'whg.27.3a', N'Whiting (Merlangius merlangus) in Division 3.a (Skagerrak and Kattegat)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'whg.27.47d', N'Whiting (Merlangius merlangus) in Subarea 4 and Division 7.d (North Sea and eastern English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'whg.27.6a', N'Whiting (Merlangius merlangus) in Division 6.a (West of Scotland)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'whg.27.6b', N'Whiting (Merlangius merlangus) in Division 6.b (Rockall)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'whg.27.7a', N'Whiting (Merlangius merlangus) in Division 7.a (Irish Sea)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'whg.27.7b-ce-k', N'Whiting (Merlangius merlangus) in Divisions 7.b-c and 7.e-k (southern Celtic Seas and eastern English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'whg.27.89a', N'Whiting (Merlangius merlangus) in Subarea 8 and Division 9.a (Bay of Biscay and Atlantic Iberian waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'wit.27.3a47d', N'Witch (Glyptocephalus cynoglossus) in Subarea 4 and Divisions 3.a and 7.d (North Sea  Skagerrak and Kattegat  eastern English Channel)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'ofj.27.nea', N'Neon flying squid (Ommastrephes bartramii) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO
INSERT [dbo].[L_Stock] ([stockCode], [description]) VALUES (N'squ.27.nea', N'Squids nei (Loliginidae, Ommastrephidae) in subareas 4-9 and divisions 2.a and 3.a (Norwegian Sea  Skagerrak and Kattegat  North Sea  Iceland and Faroes grounds  West of Scotland  Celtic Seas  Bay of Biscay  Portuguese Waters)')
GO


-- This will add the specieslist and stock relationship again if needed
--ALTER TABLE [dbo].[SpeciesList]  WITH CHECK ADD  CONSTRAINT [FK_SpeciesList_L_Stock] FOREIGN KEY([stockId])
--REFERENCES [dbo].[L_Stock] ([stockId])
--GO

--ALTER TABLE [dbo].[SpeciesList] CHECK CONSTRAINT [FK_SpeciesList_L_Stock]
--GO



/* ----- ADD NEW AquaDots TABLES ------- */




/****** Object:  Table [dbo].[L_SDAnalysisParameter]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_SDAnalysisParameter](
	[L_sdAnalysisParameterId] [int] IDENTITY(1,1) NOT NULL,
	[analysisParameter] [nvarchar](100) NOT NULL,
	[description] [nchar](500) NULL,
 CONSTRAINT [PK_L_SDAnalysisParameter] PRIMARY KEY CLUSTERED 
(
	[L_sdAnalysisParameterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[L_SDEventType]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_SDEventType](
	[L_sdEventTypeId] [int] IDENTITY(1,1) NOT NULL,
	[eventType] [nvarchar](100) NOT NULL,
	[description] [nvarchar](500) NULL,
	[ageUpdatingMethod] [nvarchar](max) NULL,
	[num] [int] NULL,
 CONSTRAINT [PK_L_SDEventType] PRIMARY KEY CLUSTERED 
(
	[L_sdEventTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[L_SDLightType]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_SDLightType](
	[L_sdLightTypeId] [int] IDENTITY(1,1) NOT NULL,
	[lightType] [nvarchar](15) NOT NULL,
	[dkDescription] [nvarchar](500) NULL,
	[ukDescription] [nvarchar](500) NULL,
 CONSTRAINT [PK_L_SDLightType] PRIMARY KEY CLUSTERED 
(
	[L_sdLightTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[L_SDOtolithDescription]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_SDOtolithDescription](
	[L_sdOtolithDescriptionId] [int] IDENTITY(1,1) NOT NULL,
	[otolithDescription] [nvarchar](15) NOT NULL,
	[dkDescription] [nvarchar](500) NULL,
	[ukDescription] [nvarchar](500) NULL,
 CONSTRAINT [PK_L_SDOtolithDescription] PRIMARY KEY CLUSTERED 
(
	[L_sdOtolithDescriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[L_SDPreparationMethod]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_SDPreparationMethod](
	[L_sdPreparationMethodId] [int] IDENTITY(1,1) NOT NULL,
	[preparationMethod] [nvarchar](10) NOT NULL,
	[dkDescription] [nvarchar](500) NULL,
	[ukDescription] [nvarchar](500) NULL,
 CONSTRAINT [PK_L_SDPreparationMethod] PRIMARY KEY CLUSTERED 
(
	[L_sdPreparationMethodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[L_SDPurpose]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_SDPurpose](
	[L_sdPurposeId] [int] IDENTITY(1,1) NOT NULL,
	[purpose] [nvarchar](100) NOT NULL,
	[description] [nvarchar](500) NULL,
 CONSTRAINT [PK_L_SDPurpose] PRIMARY KEY CLUSTERED 
(
	[L_sdPurposeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[L_SDReaderExperience]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_SDReaderExperience](
	[L_SDReaderExperienceId] [int] IDENTITY(1,1) NOT NULL,
	[readerExperience] [nvarchar](50) NOT NULL,
	[description] [nvarchar](500) NULL,
 CONSTRAINT [PK_L_SDReaderExperience] PRIMARY KEY CLUSTERED 
(
	[L_SDReaderExperienceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[L_SDSampleType]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_SDSampleType](
	[L_sdSampleTypeId] [int] IDENTITY(1,1) NOT NULL,
	[sampleType] [nvarchar](100) NOT NULL,
	[description] [nvarchar](500) NULL,
 CONSTRAINT [PK_L_SDSampleType] PRIMARY KEY CLUSTERED 
(
	[L_sdSampleTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[R_SDEventDFUArea]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[R_SDEventDFUArea](
	[sdEventId] [int] NOT NULL,
	[DFUArea] [nvarchar](3) NOT NULL,
 CONSTRAINT [PK_R_SDEventDFUArea_1] PRIMARY KEY CLUSTERED 
(
	[sdEventId] ASC,
	[DFUArea] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[R_SDEventSDReader]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[R_SDEventSDReader](
	[sdEventId] [int] NOT NULL,
	[sdReaderId] [int] NOT NULL,
	[primaryReader] [bit] NOT NULL,
 CONSTRAINT [PK_R_SDEventSDReader_1] PRIMARY KEY CLUSTERED 
(
	[sdEventId] ASC,
	[sdReaderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[R_SDReader]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[R_SDReader](
	[r_SDReaderId] [int] IDENTITY(1,1) NOT NULL,
	[dfuPersonId] [int] NOT NULL,
	[speciesCode] [nvarchar](3) NULL,
	[stockId] [int] NULL,
	[firstYearAgeReadingGeneral] [int] NULL,
	[firstYearAgeReadingCurrent] [int] NULL,
	[sdReaderExperienceId] [int] NULL,
	[sdPreparationMethodId] [int] NULL,
	[comment] [nvarchar](1000) NULL,
 CONSTRAINT [PK_R_SDReader] PRIMARY KEY CLUSTERED 
(
	[r_SDReaderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[R_StockSpeciesArea]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[R_StockSpeciesArea](
	[r_StockSpeciesAreaId] [int] IDENTITY(1,1) NOT NULL,
	[L_stockId] [int] NOT NULL,
	[DFUArea] [nvarchar](3) NOT NULL,
	[speciesCode] [nvarchar](3) NOT NULL,
	[statisticalRectangle] [dbo].[StatisticalRectangle] NULL,
	[quarter] [int] NULL,
 CONSTRAINT [PK_R_StockSpeciesArea] PRIMARY KEY CLUSTERED 
(
	[r_StockSpeciesAreaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SDAnnotation]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SDAnnotation](
	[sdAnnotationId] [int] IDENTITY(1,1) NOT NULL,
	[sdAnnotationGuid] [uniqueidentifier] NOT NULL,
	[sdFileId] [int] NOT NULL,
	[createdById] [int] NULL,
	[createdByUserName] [nvarchar](10) NULL,
	[isApproved] [bit] NULL,
	[isFixed] [bit] NULL,
	[isReadOnly] [bit] NULL,
	[createdTime] [datetime] NULL,
	[modifiedTime] [datetime] NULL,
	[sdAnalysisParameterId] [int] NULL,
	[otolithReadingRemarkId] [int] NULL,
	[edgeStructure] [nvarchar](5) NULL,
	[age] [int] NULL,
	[comment] [nvarchar](1000) NULL,
 CONSTRAINT [PK_SDAnnotation] PRIMARY KEY CLUSTERED 
(
	[sdAnnotationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SDEvent]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SDEvent](
	[sdEventId] [int] IDENTITY(1,1) NOT NULL,
	[sdEventGuid] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](200) NOT NULL,
	[speciesCode] [nvarchar](3) NULL,
	[year] [int] NULL,
	[startDate] [date] NULL,
	[endDate] [date] NULL,
	[sdPurposeId] [int] NOT NULL,
	[sdEventTypeId] [int] NOT NULL,
	[sdSampleTypeId] [int] NOT NULL,
	[createdById] [int] NULL,
	[createdByUserName] [nvarchar](10) NULL,
	[closed] [bit] NOT NULL,
	[createdTime] [datetime] NOT NULL,
	[comment] [nvarchar](1000) NULL,
	[uiSDFileExtraColumns] [nvarchar](1000) NULL,
 CONSTRAINT [PK_SDEvent] PRIMARY KEY CLUSTERED 
(
	[sdEventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SDFile]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SDFile](
	[sdFileId] [int] IDENTITY(1,1) NOT NULL,
	[sdFileGuid] [uniqueidentifier] NOT NULL,
	[sdSampleId] [int] NOT NULL,
	[fileName] [nvarchar](500) NOT NULL,
	[displayName] [nvarchar](500) NULL,
	[path] [nvarchar](500) NULL,
	[scale] [float] NULL,
	[imageWidth] [int] NULL,
	[imageHeight] [int] NULL,
 CONSTRAINT [PK_SDFile] PRIMARY KEY CLUSTERED 
(
	[sdFileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SDLine]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SDLine](
	[sdLineId] [int] IDENTITY(1,1) NOT NULL,
	[sdLineGuid] [uniqueidentifier] NOT NULL,
	[sdAnnotationId] [int] NOT NULL,
	[createdById] [int] NULL,
	[createdByUserName] [nvarchar](10) NULL,
	[createdTime] [datetime] NULL,
	[color] [nvarchar](10) NULL,
	[lineIndex] [int] NULL,
	[width] [int] NULL,
	[X1] [int] NULL,
	[X2] [int] NULL,
	[Y1] [int] NULL,
	[Y2] [int] NULL,
 CONSTRAINT [PK_SDLine] PRIMARY KEY CLUSTERED 
(
	[sdLineId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SDPoint]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SDPoint](
	[sdPointId] [int] IDENTITY(1,1) NOT NULL,
	[sdPointGuid] [uniqueidentifier] NOT NULL,
	[sdAnnotationId] [int] NOT NULL,
	[createdById] [int] NULL,
	[createdByUserName] [nvarchar](10) NULL,
	[createdTime] [datetime] NULL,
	[color] [nvarchar](10) NULL,
	[pointIndex] [int] NULL,
	[width] [int] NULL,
	[pointType] [nvarchar](200) NULL,
	[shape] [nvarchar](200) NULL,
	[X] [int] NULL,
	[Y] [int] NULL,
 CONSTRAINT [PK_SDPoint] PRIMARY KEY CLUSTERED 
(
	[sdPointId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SDSample]    Script Date: 02-08-2022 16:39:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SDSample](
	[sdSampleId] [int] IDENTITY(1,1) NOT NULL,
	[sdSampleGuid] [uniqueidentifier] NOT NULL,
	[sdEventId] [int] NOT NULL,
	[animalId] [nvarchar](50) NULL,
	[cruise] [nvarchar](20) NULL,
	[trip] [nvarchar](10) NULL,
	[station] [nvarchar](6) NULL,
	[catchDate] [datetime] NULL,
	[DFUArea] [nvarchar](3) NULL,
	[statisticalRectangle] [dbo].[StatisticalRectangle] NULL,
	[latitude] [float] NULL,
	[longitude] [float] NULL,
	[speciesCode] [nvarchar](3) NULL,
	[stockId] [int] NULL,
	[sexCode] [nvarchar](1) NULL,
	[sdPreparationMethodId] [int] NULL,
	[sdLightTypeId] [int] NULL,
	[sdOtolithDescriptionId] [int] NULL,
	[fishLengthMM] [int] NULL,
	[fishWeightG] [numeric](10, 5) NULL,
	[maturityIndexMethod] [nvarchar](1) NULL,
	[maturityId] [int] NULL,
	[otolithReadingRemarkId] [int] NULL,
	[edgeStructure] [nvarchar](5) NULL,
	[comments] [ntext] NULL,
	[createdById] [int] NULL,
	[createdByUserName] [nvarchar](10) NULL,
	[createdTime] [datetime] NULL,
	[modifiedTime] [datetime] NULL,
	[readOnly] [bit] NOT NULL,
	[importStatus] [nvarchar](100) NULL,
 CONSTRAINT [PK_SDSample] PRIMARY KEY CLUSTERED 
(
	[sdSampleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[R_SDEventDFUArea]  WITH CHECK ADD  CONSTRAINT [FK_R_SDEventDFUArea_L_DFUArea] FOREIGN KEY([DFUArea])
REFERENCES [dbo].[L_DFUArea] ([DFUArea])
GO
ALTER TABLE [dbo].[R_SDEventDFUArea] CHECK CONSTRAINT [FK_R_SDEventDFUArea_L_DFUArea]
GO
ALTER TABLE [dbo].[R_SDEventDFUArea]  WITH CHECK ADD  CONSTRAINT [FK_R_SDEventDFUArea_L_SDEventType] FOREIGN KEY([sdEventId])
REFERENCES [dbo].[SDEvent] ([sdEventId])
GO
ALTER TABLE [dbo].[R_SDEventDFUArea] CHECK CONSTRAINT [FK_R_SDEventDFUArea_L_SDEventType]
GO
ALTER TABLE [dbo].[R_SDEventSDReader]  WITH CHECK ADD  CONSTRAINT [FK_R_SDEventSDReader_R_SDReader] FOREIGN KEY([sdReaderId])
REFERENCES [dbo].[R_SDReader] ([r_SDReaderId])
GO
ALTER TABLE [dbo].[R_SDEventSDReader] CHECK CONSTRAINT [FK_R_SDEventSDReader_R_SDReader]
GO
ALTER TABLE [dbo].[R_SDEventSDReader]  WITH CHECK ADD  CONSTRAINT [FK_R_SDEventSDReader_SDEvent] FOREIGN KEY([sdEventId])
REFERENCES [dbo].[SDEvent] ([sdEventId])
GO
ALTER TABLE [dbo].[R_SDEventSDReader] CHECK CONSTRAINT [FK_R_SDEventSDReader_SDEvent]
GO
ALTER TABLE [dbo].[R_SDReader]  WITH CHECK ADD  CONSTRAINT [FK_R_SDReader_DFUPerson] FOREIGN KEY([dfuPersonId])
REFERENCES [dbo].[DFUPerson] ([dfuPersonId])
GO
ALTER TABLE [dbo].[R_SDReader] CHECK CONSTRAINT [FK_R_SDReader_DFUPerson]
GO
ALTER TABLE [dbo].[R_SDReader]  WITH CHECK ADD  CONSTRAINT [FK_R_SDReader_L_SDPreparationMethod] FOREIGN KEY([sdPreparationMethodId])
REFERENCES [dbo].[L_SDPreparationMethod] ([L_sdPreparationMethodId])
GO
ALTER TABLE [dbo].[R_SDReader] CHECK CONSTRAINT [FK_R_SDReader_L_SDPreparationMethod]
GO
ALTER TABLE [dbo].[R_SDReader]  WITH CHECK ADD  CONSTRAINT [FK_R_SDReader_L_SDReaderExperience] FOREIGN KEY([sdReaderExperienceId])
REFERENCES [dbo].[L_SDReaderExperience] ([L_SDReaderExperienceId])
GO
ALTER TABLE [dbo].[R_SDReader] CHECK CONSTRAINT [FK_R_SDReader_L_SDReaderExperience]
GO
ALTER TABLE [dbo].[R_SDReader]  WITH CHECK ADD  CONSTRAINT [FK_R_SDReader_L_Species] FOREIGN KEY([speciesCode])
REFERENCES [dbo].[L_Species] ([speciesCode])
GO
ALTER TABLE [dbo].[R_SDReader] CHECK CONSTRAINT [FK_R_SDReader_L_Species]
GO
ALTER TABLE [dbo].[R_SDReader]  WITH CHECK ADD  CONSTRAINT [FK_R_SDReader_L_Stock] FOREIGN KEY([stockId])
REFERENCES [dbo].[L_Stock] ([L_stockId])
GO
ALTER TABLE [dbo].[R_SDReader] CHECK CONSTRAINT [FK_R_SDReader_L_Stock]
GO
ALTER TABLE [dbo].[R_StockSpeciesArea]  WITH CHECK ADD  CONSTRAINT [FK_R_StockSpeciesArea_L_DFUArea] FOREIGN KEY([DFUArea])
REFERENCES [dbo].[L_DFUArea] ([DFUArea])
GO
ALTER TABLE [dbo].[R_StockSpeciesArea] CHECK CONSTRAINT [FK_R_StockSpeciesArea_L_DFUArea]
GO
ALTER TABLE [dbo].[R_StockSpeciesArea]  WITH CHECK ADD  CONSTRAINT [FK_R_StockSpeciesArea_L_Species] FOREIGN KEY([speciesCode])
REFERENCES [dbo].[L_Species] ([speciesCode])
GO
ALTER TABLE [dbo].[R_StockSpeciesArea] CHECK CONSTRAINT [FK_R_StockSpeciesArea_L_Species]
GO
ALTER TABLE [dbo].[R_StockSpeciesArea]  WITH CHECK ADD  CONSTRAINT [FK_R_StockSpeciesArea_L_StatisticalRectangle] FOREIGN KEY([statisticalRectangle])
REFERENCES [dbo].[L_StatisticalRectangle] ([statisticalRectangle])
GO
ALTER TABLE [dbo].[R_StockSpeciesArea] CHECK CONSTRAINT [FK_R_StockSpeciesArea_L_StatisticalRectangle]
GO
ALTER TABLE [dbo].[R_StockSpeciesArea]  WITH CHECK ADD  CONSTRAINT [FK_R_StockSpeciesArea_L_Stock] FOREIGN KEY([L_stockId])
REFERENCES [dbo].[L_Stock] ([L_stockId])
GO
ALTER TABLE [dbo].[R_StockSpeciesArea] CHECK CONSTRAINT [FK_R_StockSpeciesArea_L_Stock]
GO
ALTER TABLE [dbo].[SDAnnotation]  WITH CHECK ADD  CONSTRAINT [FK_SDAnnotation_DFUPerson] FOREIGN KEY([createdById])
REFERENCES [dbo].[DFUPerson] ([dfuPersonId])
GO
ALTER TABLE [dbo].[SDAnnotation] CHECK CONSTRAINT [FK_SDAnnotation_DFUPerson]
GO
ALTER TABLE [dbo].[SDAnnotation]  WITH CHECK ADD  CONSTRAINT [FK_SDAnnotation_L_EdgeStructure] FOREIGN KEY([edgeStructure])
REFERENCES [dbo].[L_EdgeStructure] ([edgeStructure])
GO
ALTER TABLE [dbo].[SDAnnotation] CHECK CONSTRAINT [FK_SDAnnotation_L_EdgeStructure]
GO
ALTER TABLE [dbo].[SDAnnotation]  WITH CHECK ADD  CONSTRAINT [FK_SDAnnotation_L_OtolithReadingRemark] FOREIGN KEY([otolithReadingRemarkId])
REFERENCES [dbo].[L_OtolithReadingRemark] ([L_OtolithReadingRemarkID])
GO
ALTER TABLE [dbo].[SDAnnotation] CHECK CONSTRAINT [FK_SDAnnotation_L_OtolithReadingRemark]
GO
ALTER TABLE [dbo].[SDAnnotation]  WITH CHECK ADD  CONSTRAINT [FK_SDAnnotation_L_SDAnalysisParameter] FOREIGN KEY([sdAnalysisParameterId])
REFERENCES [dbo].[L_SDAnalysisParameter] ([L_sdAnalysisParameterId])
GO
ALTER TABLE [dbo].[SDAnnotation] CHECK CONSTRAINT [FK_SDAnnotation_L_SDAnalysisParameter]
GO
ALTER TABLE [dbo].[SDAnnotation]  WITH NOCHECK ADD  CONSTRAINT [FK_SDAnnotation_SDFile] FOREIGN KEY([sdFileId])
REFERENCES [dbo].[SDFile] ([sdFileId])
GO
ALTER TABLE [dbo].[SDAnnotation] CHECK CONSTRAINT [FK_SDAnnotation_SDFile]
GO
ALTER TABLE [dbo].[SDEvent]  WITH CHECK ADD  CONSTRAINT [FK_SDEvent_DFUPerson] FOREIGN KEY([createdById])
REFERENCES [dbo].[DFUPerson] ([dfuPersonId])
GO
ALTER TABLE [dbo].[SDEvent] CHECK CONSTRAINT [FK_SDEvent_DFUPerson]
GO
ALTER TABLE [dbo].[SDEvent]  WITH CHECK ADD  CONSTRAINT [FK_SDEvent_L_SDEventType] FOREIGN KEY([sdEventTypeId])
REFERENCES [dbo].[L_SDEventType] ([L_sdEventTypeId])
GO
ALTER TABLE [dbo].[SDEvent] CHECK CONSTRAINT [FK_SDEvent_L_SDEventType]
GO
ALTER TABLE [dbo].[SDEvent]  WITH CHECK ADD  CONSTRAINT [FK_SDEvent_L_SDPurpose] FOREIGN KEY([sdPurposeId])
REFERENCES [dbo].[L_SDPurpose] ([L_sdPurposeId])
GO
ALTER TABLE [dbo].[SDEvent] CHECK CONSTRAINT [FK_SDEvent_L_SDPurpose]
GO
ALTER TABLE [dbo].[SDEvent]  WITH CHECK ADD  CONSTRAINT [FK_SDEvent_L_SDSampleType] FOREIGN KEY([sdSampleTypeId])
REFERENCES [dbo].[L_SDSampleType] ([L_sdSampleTypeId])
GO
ALTER TABLE [dbo].[SDEvent] CHECK CONSTRAINT [FK_SDEvent_L_SDSampleType]
GO
ALTER TABLE [dbo].[SDEvent]  WITH CHECK ADD  CONSTRAINT [FK_SDEvent_L_Species] FOREIGN KEY([speciesCode])
REFERENCES [dbo].[L_Species] ([speciesCode])
GO
ALTER TABLE [dbo].[SDEvent] CHECK CONSTRAINT [FK_SDEvent_L_Species]
GO
ALTER TABLE [dbo].[SDFile]  WITH NOCHECK ADD  CONSTRAINT [FK_SDFile_SDSample] FOREIGN KEY([sdSampleId])
REFERENCES [dbo].[SDSample] ([sdSampleId])
GO
ALTER TABLE [dbo].[SDFile] CHECK CONSTRAINT [FK_SDFile_SDSample]
GO
ALTER TABLE [dbo].[SDLine]  WITH NOCHECK ADD  CONSTRAINT [FK_SDLine_DFUPerson] FOREIGN KEY([createdById])
REFERENCES [dbo].[DFUPerson] ([dfuPersonId])
GO
ALTER TABLE [dbo].[SDLine] CHECK CONSTRAINT [FK_SDLine_DFUPerson]
GO
ALTER TABLE [dbo].[SDLine]  WITH CHECK ADD  CONSTRAINT [FK_SDLine_SDAnnotation] FOREIGN KEY([sdAnnotationId])
REFERENCES [dbo].[SDAnnotation] ([sdAnnotationId])
GO
ALTER TABLE [dbo].[SDLine] CHECK CONSTRAINT [FK_SDLine_SDAnnotation]
GO
ALTER TABLE [dbo].[SDPoint]  WITH CHECK ADD  CONSTRAINT [FK_SDPoint_DFUPerson] FOREIGN KEY([createdById])
REFERENCES [dbo].[DFUPerson] ([dfuPersonId])
GO
ALTER TABLE [dbo].[SDPoint] CHECK CONSTRAINT [FK_SDPoint_DFUPerson]
GO
ALTER TABLE [dbo].[SDPoint]  WITH NOCHECK ADD  CONSTRAINT [FK_SDPoint_SDAnnotation] FOREIGN KEY([sdAnnotationId])
REFERENCES [dbo].[SDAnnotation] ([sdAnnotationId])
GO
ALTER TABLE [dbo].[SDPoint] CHECK CONSTRAINT [FK_SDPoint_SDAnnotation]
GO
ALTER TABLE [dbo].[SDSample]  WITH CHECK ADD  CONSTRAINT [FK_SDSample_DFUPerson] FOREIGN KEY([createdById])
REFERENCES [dbo].[DFUPerson] ([dfuPersonId])
GO
ALTER TABLE [dbo].[SDSample] CHECK CONSTRAINT [FK_SDSample_DFUPerson]
GO
ALTER TABLE [dbo].[SDSample]  WITH CHECK ADD  CONSTRAINT [FK_SDSample_L_DFUArea] FOREIGN KEY([DFUArea])
REFERENCES [dbo].[L_DFUArea] ([DFUArea])
GO
ALTER TABLE [dbo].[SDSample] CHECK CONSTRAINT [FK_SDSample_L_DFUArea]
GO
ALTER TABLE [dbo].[SDSample]  WITH CHECK ADD  CONSTRAINT [FK_SDSample_L_EdgeStructure] FOREIGN KEY([edgeStructure])
REFERENCES [dbo].[L_EdgeStructure] ([edgeStructure])
GO
ALTER TABLE [dbo].[SDSample] CHECK CONSTRAINT [FK_SDSample_L_EdgeStructure]
GO
ALTER TABLE [dbo].[SDSample]  WITH CHECK ADD  CONSTRAINT [FK_SDSample_L_MaturityIndexMethod] FOREIGN KEY([maturityIndexMethod])
REFERENCES [dbo].[L_MaturityIndexMethod] ([maturityIndexMethod])
GO
ALTER TABLE [dbo].[SDSample] CHECK CONSTRAINT [FK_SDSample_L_MaturityIndexMethod]
GO
ALTER TABLE [dbo].[SDSample]  WITH CHECK ADD  CONSTRAINT [FK_SDSample_L_OtolithReadingRemark] FOREIGN KEY([otolithReadingRemarkId])
REFERENCES [dbo].[L_OtolithReadingRemark] ([L_OtolithReadingRemarkID])
GO
ALTER TABLE [dbo].[SDSample] CHECK CONSTRAINT [FK_SDSample_L_OtolithReadingRemark]
GO
ALTER TABLE [dbo].[SDSample]  WITH CHECK ADD  CONSTRAINT [FK_SDSample_L_SDLightType] FOREIGN KEY([sdLightTypeId])
REFERENCES [dbo].[L_SDLightType] ([L_sdLightTypeId])
GO
ALTER TABLE [dbo].[SDSample] CHECK CONSTRAINT [FK_SDSample_L_SDLightType]
GO
ALTER TABLE [dbo].[SDSample]  WITH CHECK ADD  CONSTRAINT [FK_SDSample_L_SDOtolithDescription] FOREIGN KEY([sdOtolithDescriptionId])
REFERENCES [dbo].[L_SDOtolithDescription] ([L_sdOtolithDescriptionId])
GO
ALTER TABLE [dbo].[SDSample] CHECK CONSTRAINT [FK_SDSample_L_SDOtolithDescription]
GO
ALTER TABLE [dbo].[SDSample]  WITH CHECK ADD  CONSTRAINT [FK_SDSample_L_SDPreparationMethod] FOREIGN KEY([sdPreparationMethodId])
REFERENCES [dbo].[L_SDPreparationMethod] ([L_sdPreparationMethodId])
GO
ALTER TABLE [dbo].[SDSample] CHECK CONSTRAINT [FK_SDSample_L_SDPreparationMethod]
GO
ALTER TABLE [dbo].[SDSample]  WITH CHECK ADD  CONSTRAINT [FK_SDSample_L_SexCode] FOREIGN KEY([sexCode])
REFERENCES [dbo].[L_SexCode] ([sexCode])
GO
ALTER TABLE [dbo].[SDSample] CHECK CONSTRAINT [FK_SDSample_L_SexCode]
GO
ALTER TABLE [dbo].[SDSample]  WITH CHECK ADD  CONSTRAINT [FK_SDSample_L_Species] FOREIGN KEY([speciesCode])
REFERENCES [dbo].[L_Species] ([speciesCode])
GO
ALTER TABLE [dbo].[SDSample] CHECK CONSTRAINT [FK_SDSample_L_Species]
GO
ALTER TABLE [dbo].[SDSample]  WITH CHECK ADD  CONSTRAINT [FK_SDSample_L_StatisticalRectangle] FOREIGN KEY([statisticalRectangle])
REFERENCES [dbo].[L_StatisticalRectangle] ([statisticalRectangle])
GO
ALTER TABLE [dbo].[SDSample] CHECK CONSTRAINT [FK_SDSample_L_StatisticalRectangle]
GO
ALTER TABLE [dbo].[SDSample]  WITH CHECK ADD  CONSTRAINT [FK_SDSample_L_Stock] FOREIGN KEY([stockId])
REFERENCES [dbo].[L_Stock] ([L_stockId])
GO
ALTER TABLE [dbo].[SDSample] CHECK CONSTRAINT [FK_SDSample_L_Stock]
GO
ALTER TABLE [dbo].[SDSample]  WITH CHECK ADD  CONSTRAINT [FK_SDSample_Maturity] FOREIGN KEY([maturityId])
REFERENCES [dbo].[Maturity] ([maturityId])
GO
ALTER TABLE [dbo].[SDSample] CHECK CONSTRAINT [FK_SDSample_Maturity]
GO
ALTER TABLE [dbo].[SDSample]  WITH NOCHECK ADD  CONSTRAINT [FK_SDSample_SDEvent] FOREIGN KEY([sdEventId])
REFERENCES [dbo].[SDEvent] ([sdEventId])
GO
ALTER TABLE [dbo].[SDSample] CHECK CONSTRAINT [FK_SDSample_SDEvent]
GO





-- Create index for R_SDEventDFUArea.

CREATE NONCLUSTERED INDEX [IX_R_SDEventDFUArea] ON [dbo].[R_SDEventDFUArea]
(
	[DFUArea] ASC,
	[sdEventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO


-- Add sdAgeInfoUpdated column.
ALTER TABLE [dbo].[Age]
ADD [sdAgeInfoUpdated] bit NULL
GO

-- Add sdAgeReadId column.
ALTER TABLE [dbo].[Age]
ADD [sdAgeReadId] int NULL
GO


-- Add sdAnnotationId column.
ALTER TABLE [dbo].[Age]
ADD [sdAnnotationId] int  NULL
GO


-- Creating foreign key on [sdAgeReadId] in table 'Age'
ALTER TABLE [dbo].[Age]
ADD CONSTRAINT [FK_Age_DFUPerson] FOREIGN KEY ([sdAgeReadId]) REFERENCES [dbo].[DFUPerson] ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Age_DFUPerson'
CREATE INDEX [IX_FK_Age_DFUPerson]
ON [dbo].[Age] ([sdAgeReadId]);
GO


-- Creating foreign key on [sdAnnotationId] in table 'Age'
ALTER TABLE [dbo].[Age]
ADD CONSTRAINT [FK_Age_SDAnnotation] FOREIGN KEY ([sdAnnotationId]) REFERENCES [dbo].[SDAnnotation] ([sdAnnotationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Age_SDAnnotation'
CREATE INDEX [IX_FK_Age_SDAnnotation]
ON [dbo].[Age] ([sdAnnotationId]);
GO

-- Change DFUPerson initials from 4 to 10
ALTER TABLE [dbo].[DFUPerson]
ALTER COLUMN [initials] [nvarchar](10) NOT NULL
GO


ALTER TABLE [dbo].[SDEvent]
ADD [defaultImageFolders] nvarchar(max)  NULL
GO



INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('ALX' , 'Hele ubehandlet' , 'Whole untreated')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('ALE' , 'Hele i entellan' , 'Whole in entellan')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('ALP' , 'Hele og slebet' , 'Whole and polished')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('ALR' , 'Hele i resin' , 'Whole in resin')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('ALW' , 'Hele i vand' , 'Whole in water')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('ALA' , 'Hele i alkohol' , 'Whole in alcohol')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('BBX' , 'Knækkede og brændt' , 'Broken and burnt')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('BPX' , 'Knækkede og slebet' , 'Broken and polished')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('BRX' , 'Knækkede' , 'Broken')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('BUX' , 'Brændt' , 'Burnt')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('CDX' , 'Rensede og tørret' , 'Cleaned and dried')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('DXX' , 'Tørret' , 'Dried')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('GCS' , 'Ground to a cross-section' , 'Ground to a cross-section')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('GHX' , 'Ground horizontally' , 'Ground horizontally')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('GRX' , 'Ground' , 'Ground')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('GSX' , 'Ground and stained' , 'Ground and stained')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('GRP' , 'Ground and polished' , 'Ground and polished')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('IMP' , 'Aftryk' , 'Impression')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('IAS' , 'Aftryk på acetate slides' , 'Impression on acetate slides')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('IIM' , 'Aftryk og fotograferet' , 'Impression and imaged')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('PXX' , 'Slebet' , 'Polished')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('SEX' , 'Sektioneret' , 'Sectioned')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('SPX' , 'Sektioneret og slebet' , 'Sectioned and polished')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('SSX' , 'Sektioneret og farvet' , 'Sectioned and stained')
GO
INSERT INTO [dbo].[L_SDPreparationMethod] ([preparationMethod] ,[dkDescription],[ukDescription]) VALUES ('STX' , 'Farvet' , 'Stained')
GO


INSERT INTO [dbo].[L_SDLightType] ([lightType],[dkDescription] ,[ukDescription]) VALUES ('RLX', 'Overlys', 'Reflected light')
GO
INSERT INTO [dbo].[L_SDLightType] ([lightType],[dkDescription] ,[ukDescription]) VALUES ('TLX', 'Underlys', 'Transmitted light')
GO
INSERT INTO [dbo].[L_SDLightType] ([lightType],[dkDescription] ,[ukDescription]) VALUES ('TRL', 'Over og underlys', 'Transmitted and reflected light')
GO


INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('XX', 'Begge ok', 'Both ok')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('X0', 'Venstre ok – højre mangler', 'Left ok – right missing')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('0X', 'Venstre mangler – højre ok', 'Left missing – right ok')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('00', 'Begge mangler', 'Both missing')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('XC', 'Venstre ok – højre krystalliseret', 'Left ok – right crystalized')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('CX', 'Venstre krystalliseret – højre ok', 'Left crystalized – right ok')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('CC', 'Begge krystalliseret', 'Both crystalized')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('0C', 'Venstre mangler – højre krystalliseret', 'Left missing – right crystalized')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('C0', 'Venstre krystalliseret – højre mangler', 'Left crystalized –right missing')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('XB', 'Venstre ok – højre knækket', 'Left ok - right broken')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('BX', 'Venstre knækket - højre ok', 'Left broken - right ok')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('BB', 'Begge knækket', 'Both broken')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('0B', 'Venstre mangler – højre knækket', 'Left missing – right broken')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('B0', 'Venstre knækket – højre mangler', 'Left broken – right missing')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('XD', 'Venstre ok – højre beskidt', 'Left ok - right dirty')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('DX', 'Venstre beskidt – højre ok', 'Left dirty - right ok')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('DD', 'Begge beskidt', 'Both dirty')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('0D', 'Venstre mangler – højre beskidt', 'Left missing – right dirty')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('D0', 'Venstre beskidt – højre mangler', 'Left dirty – right missing')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('XT', 'Venstre ok – højre krystalliseret og knækket', 'Left ok – right crystalized and broken')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('TX', 'Venstre krystalliseret og knækket – højre ok', 'Left crystalized and broken – right ok')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('TT', 'Begge krystalliseret og knækket', 'Both crystalized and broken')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('0T', 'Venstre mangler – højre krystalliseret og knækket', 'Left missing - right crystalized and broken')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('T0', 'Venstre krystalliseret og knækket – højre mangler', 'Left crystalized and broken – right missing')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('XG', 'Venstre ok – højre limet', 'Left ok – right glued')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('GX', 'Venstre limet – højre ok', 'Left glued – right ok')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('GG', 'Begge limet', 'Both glued')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('0G', 'Venstre mangler – højre limet', 'Left missing – right glued')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('G0', 'Venstre limet – højre mangler', 'Left glued – right missing')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('XM', 'Venstre ok – højre misformet', 'Left ok – right misshaped')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('MX', 'Venstre misformet – højre ok', 'Left misshaped – right ok')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('MM', 'Begge misformet', 'Both misshaped')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('0M', 'Venstre mangler – højre misformet', 'Left missing – right misshaped')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('M0', 'Venstre misformet – højre mangler', 'Left misshaped – right missing')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('SS', 'Begge rystet', 'Both shaken')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('UN', 'Kun 1 otolit, og uvist om den er venstre eller højre', 'Only 1 otolith and don’t know if it is left or right')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('UC', 'Kun 1 otolit, uvist om det er venstre eller højre, og krystalliseret', 'Only 1 otolith and  unknown if left or right, but crystalized')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('UB', 'Kun 1 otolit, uvist om det er venstre eller højre, og knækket', 'Only 1 otolith and unknown if left or right, but broken')
GO
INSERT INTO [dbo].[L_SDOtolithDescription]([otolithDescription],[dkDescription],[ukDescription]) VALUES ('FF', 'Begge oto vender forkert (sulcus forkert)', 'Both left and right otoliths flipped (sulcus wrong)')
GO

INSERT INTO [dbo].[L_SDSampleType] ([sampleType], [description]) VALUES ('Gonad', 'Gonad')
GO
INSERT INTO [dbo].[L_SDSampleType] ([sampleType], [description]) VALUES ('Otolith', 'Otolit')
GO
INSERT INTO [dbo].[L_SDSampleType] ([sampleType], [description]) VALUES ('Scale', 'Scale')
GO
INSERT INTO [dbo].[L_SDSampleType] ([sampleType], [description]) VALUES ('Vertebra', 'Vertebra')
GO
INSERT INTO [dbo].[L_SDSampleType] ([sampleType], [description]) VALUES ('Egg', 'Egg')
GO
INSERT INTO [dbo].[L_SDSampleType] ([sampleType], [description]) VALUES ('Larvae', 'Larvae')
GO



INSERT [dbo].[L_SDPurpose] ([purpose], [description]) VALUES (N'AR', N'Aldersaflæsning')
GO


INSERT [dbo].[L_SDEventType] ([eventType], [description], [ageUpdatingMethod], [num]) VALUES (N'ARS', N'Årsaflæsning', N'UpdateAges', 1)
GO

INSERT [dbo].[L_SDEventType] ([eventType], [description], [ageUpdatingMethod], [num]) VALUES (N'SAM', N'Sammenlæsning', N'NeverUpdateAges', 2)
GO

INSERT [dbo].[L_SDEventType] ([eventType], [description], [ageUpdatingMethod], [num]) VALUES (N'REF', N'Reference', N'NeverUpdateAges', 3)
GO


INSERT INTO [dbo].[L_SDReaderExperience] ([readerExperience],[description]) VALUES ('ADV' , 'Advanced')
GO
INSERT INTO [dbo].[L_SDReaderExperience] ([readerExperience],[description]) VALUES ('BAS' , 'Basic')
GO


INSERT INTO [dbo].[L_SDAnalysisParameter] ([analysisParameter] ,[description])  VALUES ('OWR', 'Age, method: Otolith Winter Rings')
GO



SET IDENTITY_INSERT [dbo].[R_StockSpeciesArea] ON 
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1, 4, N'8', N'ANS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (2, 4, N'8A', N'ANS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (3, 4, N'8B', N'ANS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (4, 4, N'8C', N'ANS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (5, 4, N'8D', N'ANS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (6, 4, N'8E', N'ANS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (7, 5, N'9A', N'ANS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (8, 8, N'7', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (9, 8, N'7A', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (10, 8, N'7B', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (11, 8, N'7C', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (12, 8, N'7D', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (13, 8, N'7E', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (14, 8, N'7F', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (15, 8, N'7G', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (16, 8, N'7H', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (17, 8, N'7J', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (18, 8, N'7K', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (19, 8, N'8A', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (20, 8, N'8B', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (21, 8, N'8D', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (22, 9, N'8C', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (23, 9, N'9A', N'HAS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (24, 10, N'1', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (25, 10, N'2', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (26, 10, N'2A', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (27, 10, N'2B', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (28, 10, N'3A', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (29, 10, N'20', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (30, 10, N'21', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (31, 10, N'4', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (32, 10, N'4A', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (33, 10, N'4B', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (34, 10, N'4C', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (35, 11, N'14', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (36, 11, N'14B', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (37, 11, N'5A', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (38, 12, N'5B', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (39, 12, N'5B1', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (40, 12, N'5B2', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (41, 12, N'6A', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (42, 13, N'10', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (43, 13, N'12', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (44, 13, N'6B', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (45, 13, N'7', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (46, 13, N'7A', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (47, 13, N'7B', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (48, 13, N'7C', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (49, 13, N'7D', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (50, 13, N'7E', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (51, 13, N'7F', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (52, 13, N'7G', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (53, 13, N'7H', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (54, 13, N'7J', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (55, 13, N'7K', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (56, 13, N'8', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (57, 13, N'8A', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (58, 13, N'8B', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (59, 13, N'8C', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (60, 13, N'8D', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (61, 13, N'8E', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (62, 13, N'9', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (63, 13, N'9A', N'GUK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (64, 17, N'23', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (65, 17, N'22', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (66, 17, N'3D', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (67, 17, N'24', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (68, 17, N'25', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (69, 17, N'26', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (70, 17, N'27', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (71, 17, N'28', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (72, 17, N'29', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (73, 17, N'30', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (74, 17, N'31', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (75, 17, N'32', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (76, 18, N'3A', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (77, 18, N'20', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (78, 18, N'21', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (79, 18, N'4', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (80, 18, N'4A', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (81, 18, N'4B', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (82, 18, N'4C', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (83, 18, N'7D', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (84, 18, N'7E', N'SLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (85, 21, N'1', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (86, 21, N'10', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (87, 21, N'12', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (88, 21, N'14', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (89, 21, N'14B', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (90, 21, N'2', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (91, 21, N'2A', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (92, 21, N'2B', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (93, 21, N'3', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (94, 21, N'3A', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (95, 21, N'20', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (96, 21, N'21', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (97, 21, N'23', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (98, 21, N'22', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (99, 21, N'3D', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (100, 21, N'24', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (101, 21, N'25', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (102, 21, N'26', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (103, 21, N'27', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (104, 21, N'28', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (105, 21, N'29', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (106, 21, N'30', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (107, 21, N'31', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (108, 21, N'32', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (109, 21, N'4', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (110, 21, N'4A', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (111, 21, N'4B', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (112, 21, N'4C', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (113, 21, N'5', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (114, 21, N'5A', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (115, 21, N'5B', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (116, 21, N'5B1', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (117, 21, N'5B2', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (118, 21, N'6', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (119, 21, N'6A', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (120, 21, N'6AN', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (121, 21, N'6AS', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (122, 21, N'6B', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (123, 21, N'7', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (124, 21, N'7A', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (125, 21, N'7B', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (126, 21, N'7C', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (127, 21, N'7D', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (128, 21, N'7E', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (129, 21, N'7F', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (130, 21, N'7G', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (131, 21, N'7H', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (132, 21, N'7J', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (133, 21, N'7K', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (134, 21, N'8', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (135, 21, N'8A', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (136, 21, N'8B', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (137, 21, N'8C', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (138, 21, N'8D', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (139, 21, N'8E', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (140, 21, N'9', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (141, 21, N'9A', N'BRU', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (142, 30, N'3A', N'HAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (143, 30, N'20', N'HAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (144, 30, N'21', N'HAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (145, 30, N'4', N'HAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (146, 30, N'4A', N'HAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (147, 30, N'4B', N'HAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (148, 30, N'4C', N'HAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (149, 30, N'7D', N'HAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (150, 30, N'7E', N'HAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (151, 31, N'1', N'LOD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (152, 31, N'2B', N'LOD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (153, 32, N'14', N'LOD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (154, 32, N'14B', N'LOD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (155, 32, N'5', N'LOD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (156, 32, N'5A', N'LOD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (157, 32, N'5B', N'LOD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (158, 32, N'5B1', N'LOD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (159, 32, N'5B2', N'LOD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (160, 35, N'14', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (161, 35, N'14B', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (162, 36, N'1', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (163, 36, N'2', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (164, 36, N'2A', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (165, 36, N'2B', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (166, 38, N'21', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (167, 39, N'23', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (168, 39, N'22', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (169, 39, N'24', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (170, 40, N'25', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (171, 40, N'26', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (172, 40, N'27', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (173, 40, N'28', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (174, 40, N'29', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (175, 40, N'30', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (176, 40, N'31', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (177, 40, N'32', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (178, 41, N'20', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (179, 41, N'4', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (180, 41, N'4A', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (181, 41, N'4B', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (182, 41, N'4C', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (183, 41, N'7D', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (184, 42, N'5A', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (185, 43, N'5B1', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (186, 44, N'5B2', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (187, 45, N'6A', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (188, 45, N'6AN', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (189, 45, N'6AS', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (190, 46, N'6B', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (191, 47, N'7A', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (192, 48, N'7E', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (193, 48, N'7F', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (194, 48, N'7G', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (195, 48, N'7H', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (196, 48, N'7J', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (197, 48, N'7K', N'TOR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (198, 49, N'3A', N'HAA', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (199, 49, N'20', N'HAA', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (200, 49, N'21', N'HAA', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (201, 49, N'4', N'HAA', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (202, 49, N'4A', N'HAA', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (203, 49, N'4B', N'HAA', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (204, 49, N'4C', N'HAA', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (205, 49, N'7D', N'HAA', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (206, 49, N'7E', N'HAA', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (207, 54, N'23', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (208, 54, N'22', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (209, 54, N'3D', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (210, 54, N'24', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (211, 54, N'25', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (212, 54, N'26', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (213, 54, N'27', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (214, 54, N'28', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (215, 54, N'29', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (216, 54, N'30', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (217, 54, N'31', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (218, 54, N'32', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (219, 55, N'3A', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (220, 55, N'20', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (221, 55, N'21', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (222, 55, N'4', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (223, 55, N'4A', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (224, 55, N'4B', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (225, 55, N'4C', N'ISG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (226, 56, N'1', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (227, 56, N'10', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (228, 56, N'12', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (229, 56, N'14', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (230, 56, N'14B', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (231, 56, N'2', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (232, 56, N'2A', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (233, 56, N'2B', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (234, 56, N'3', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (235, 56, N'3A', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (236, 56, N'20', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (237, 56, N'21', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (238, 56, N'23', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (239, 56, N'22', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (240, 56, N'3D', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (241, 56, N'24', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (242, 56, N'25', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (243, 56, N'26', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (244, 56, N'27', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (245, 56, N'28', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (246, 56, N'29', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (247, 56, N'30', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (248, 56, N'31', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (249, 56, N'32', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (250, 56, N'4', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (251, 56, N'4A', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (252, 56, N'4B', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (253, 56, N'4C', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (254, 56, N'5', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (255, 56, N'5A', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (256, 56, N'5B', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (257, 56, N'5B1', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (258, 56, N'5B2', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (259, 56, N'6', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (260, 56, N'6A', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (261, 56, N'6B', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (262, 56, N'7', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (263, 56, N'7A', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (264, 56, N'7B', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (265, 56, N'7C', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (266, 56, N'7D', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (267, 56, N'7E', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (268, 56, N'7F', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (269, 56, N'7G', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (270, 56, N'7H', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (271, 56, N'7J', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (272, 56, N'7K', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (273, 56, N'8', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (274, 56, N'8A', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (275, 56, N'8B', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (276, 56, N'8C', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (277, 56, N'8D', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (278, 56, N'8E', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (279, 56, N'9', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (280, 56, N'9A', N'PHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (281, 59, N'1', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (282, 59, N'10', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (283, 59, N'12', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (284, 59, N'14', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (285, 59, N'14B', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (286, 59, N'2', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (287, 59, N'2A', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (288, 59, N'2B', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (289, 59, N'3', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (290, 59, N'3A', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (291, 59, N'20', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (292, 59, N'21', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (293, 59, N'23', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (294, 59, N'22', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (295, 59, N'3D', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (296, 59, N'24', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (297, 59, N'25', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (298, 59, N'26', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (299, 59, N'27', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (300, 59, N'28', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (301, 59, N'29', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (302, 59, N'30', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (303, 59, N'31', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (304, 59, N'32', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (305, 59, N'4', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (306, 59, N'4A', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (307, 59, N'4B', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (308, 59, N'4C', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (309, 59, N'5', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (310, 59, N'5A', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (311, 59, N'5B', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (312, 59, N'5B1', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (313, 59, N'5B2', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (314, 59, N'6', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (315, 59, N'6A', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (316, 59, N'6B', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (317, 59, N'7', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (318, 59, N'7A', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (319, 59, N'7B', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (320, 59, N'7C', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (321, 59, N'7D', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (322, 59, N'7E', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (323, 59, N'7F', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (324, 59, N'7G', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (325, 59, N'7H', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (326, 59, N'7J', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (327, 59, N'7K', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (328, 59, N'8', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (329, 59, N'8A', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (330, 59, N'8B', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (331, 59, N'8C', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (332, 59, N'8D', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (333, 59, N'8E', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (334, 59, N'9', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (335, 59, N'9A', N'GLL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (336, 61, N'23', N'SKR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (337, 61, N'22', N'SKR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (338, 62, N'3A', N'SKR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (339, 62, N'20', N'SKR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (340, 62, N'21', N'SKR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (341, 62, N'4', N'SKR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (342, 62, N'4A', N'SKR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (343, 62, N'4B', N'SKR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (344, 62, N'4C', N'SKR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (345, 63, N'1', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (346, 63, N'10', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (347, 63, N'12', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (348, 63, N'14', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (349, 63, N'14B', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (350, 63, N'2', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (351, 63, N'2A', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (352, 63, N'2B', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (353, 63, N'3', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (354, 63, N'3A', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (355, 63, N'20', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (356, 63, N'21', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (357, 63, N'23', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (358, 63, N'22', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (359, 63, N'3D', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (360, 63, N'24', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (361, 63, N'25', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (362, 63, N'26', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (363, 63, N'27', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (364, 63, N'28', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (365, 63, N'29', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (366, 63, N'30', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (367, 63, N'31', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (368, 63, N'32', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (369, 63, N'4', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (370, 63, N'4A', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (371, 63, N'4B', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (372, 63, N'4C', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (373, 63, N'5', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (374, 63, N'5A', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (375, 63, N'5B', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (376, 63, N'5B1', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (377, 63, N'5B2', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (378, 63, N'6', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (379, 63, N'6A', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (380, 63, N'6AN', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (381, 63, N'6AS', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (382, 63, N'6B', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (383, 63, N'7', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (384, 63, N'7A', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (385, 63, N'7B', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (386, 63, N'7C', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (387, 63, N'7D', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (388, 63, N'7E', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (389, 63, N'7F', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (390, 63, N'7G', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (391, 63, N'7H', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (392, 63, N'7J', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (393, 63, N'7K', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (394, 63, N'8', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (395, 63, N'8A', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (396, 63, N'8B', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (397, 63, N'8C', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (398, 63, N'8D', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (399, 63, N'8E', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (400, 63, N'9', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (401, 63, N'9A', N'GHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (402, 64, N'1', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (403, 64, N'10', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (404, 64, N'12', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (405, 64, N'2', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (406, 64, N'2A', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (407, 64, N'2B', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (408, 64, N'3', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (409, 64, N'3A', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (410, 64, N'20', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (411, 64, N'21', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (412, 64, N'23', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (413, 64, N'22', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (414, 64, N'3D', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (415, 64, N'24', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (416, 64, N'25', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (417, 64, N'26', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (418, 64, N'27', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (419, 64, N'28', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (420, 64, N'29', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (421, 64, N'30', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (422, 64, N'31', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (423, 64, N'32', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (424, 64, N'4', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (425, 64, N'4A', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (426, 64, N'4B', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (427, 64, N'4C', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (428, 64, N'5', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (429, 64, N'5A', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (430, 64, N'5B', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (431, 64, N'5B1', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (432, 64, N'5B2', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (433, 64, N'6', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (434, 64, N'6A', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (435, 64, N'6AN', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (436, 64, N'6AS', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (437, 64, N'6B', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (438, 64, N'7', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (439, 64, N'7A', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (440, 64, N'7B', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (441, 64, N'7C', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (442, 64, N'7D', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (443, 64, N'7E', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (444, 64, N'7F', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (445, 64, N'7G', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (446, 64, N'7H', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (447, 64, N'7J', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (448, 64, N'7K', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (449, 64, N'8', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (450, 64, N'8A', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (451, 64, N'8B', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (452, 64, N'8C', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (453, 64, N'8D', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (454, 64, N'8E', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (455, 64, N'9', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (456, 64, N'9A', N'SBE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (457, 68, N'3A', N'KNH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (458, 68, N'20', N'KNH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (459, 68, N'21', N'KNH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (460, 68, N'4', N'KNH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (461, 68, N'4A', N'KNH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (462, 68, N'4B', N'KNH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (463, 68, N'4C', N'KNH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (464, 68, N'7D', N'KNH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (465, 70, N'3', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (466, 70, N'3A', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (467, 70, N'20', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (468, 70, N'21', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (469, 70, N'23', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (470, 70, N'22', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (471, 70, N'3D', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (472, 70, N'24', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (473, 70, N'25', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (474, 70, N'26', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (475, 70, N'27', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (476, 70, N'28', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (477, 70, N'29', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (478, 70, N'30', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (479, 70, N'31', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (480, 70, N'32', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (481, 70, N'4', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (482, 70, N'4A', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (483, 70, N'4B', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (484, 70, N'4C', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (485, 70, N'5', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (486, 70, N'5A', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (487, 70, N'5B', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (488, 70, N'5B1', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (489, 70, N'5B2', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (490, 70, N'6', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (491, 70, N'6A', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (492, 70, N'6B', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (493, 70, N'7', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (494, 70, N'7A', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (495, 70, N'7B', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (496, 70, N'7C', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (497, 70, N'7D', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (498, 70, N'7E', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (499, 70, N'7F', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (500, 70, N'7G', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (501, 70, N'7H', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (502, 70, N'7J', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (503, 70, N'7K', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (504, 70, N'8', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (505, 70, N'8A', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (506, 70, N'8B', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (507, 70, N'8C', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (508, 70, N'8D', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (509, 70, N'8E', N'RKH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (510, 71, N'1', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (511, 71, N'2', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (512, 71, N'2A', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (513, 71, N'2B', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (514, 72, N'20', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (515, 72, N'4', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (516, 72, N'4A', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (517, 72, N'4B', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (518, 72, N'4C', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (519, 72, N'6A', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (520, 72, N'6AN', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (521, 72, N'6AS', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (522, 73, N'5A', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (523, 74, N'5B', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (524, 74, N'5B1', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (525, 74, N'5B2', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (526, 75, N'6B', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (527, 76, N'7A', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (528, 77, N'7B', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (529, 77, N'7C', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (530, 77, N'7D', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (531, 77, N'7E', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (532, 77, N'7F', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (533, 77, N'7G', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (534, 77, N'7H', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (535, 77, N'7J', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (536, 77, N'7K', N'KUL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (537, 78, N'3A', N'HLF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (538, 78, N'20', N'HLF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (539, 78, N'21', N'HLF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (540, 78, N'4', N'HLF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (541, 78, N'4A', N'HLF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (542, 78, N'4B', N'HLF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (543, 78, N'4C', N'HLF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (544, 78, N'7D', N'HLF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (545, 78, N'7E', N'HLF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (546, 79, N'1', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (547, 79, N'2', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (548, 79, N'2A', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (549, 79, N'2B', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (550, 79, N'5', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (551, 79, N'5B', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (552, 79, N'5B1', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (553, 79, N'5B2', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (554, 80, N'3A', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (555, 80, N'20', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (556, 80, N'21', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (557, 80, N'23', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (558, 80, N'22', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (559, 80, N'24', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (560, 81, N'25', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (561, 81, N'26', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (562, 81, N'27', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (563, 81, N'29', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (564, 81, N'32', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (565, 83, N'30', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (566, 83, N'31', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (567, 84, N'4', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (568, 84, N'4A', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (569, 84, N'4B', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (570, 84, N'4C', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (571, 84, N'7D', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (572, 85, N'5A', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (573, 86, N'6AN', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (574, 86, N'6AS', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (575, 86, N'7B', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (576, 86, N'7C', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (577, 87, N'7G', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (578, 87, N'7H', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (579, 87, N'7J', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (580, 87, N'7K', N'SIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (581, 89, N'3A', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (582, 89, N'20', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (583, 89, N'21', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (584, 89, N'4', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (585, 89, N'4A', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (586, 89, N'4B', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (587, 89, N'4C', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (588, 89, N'5B', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (589, 89, N'5B1', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (590, 89, N'5B2', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (591, 89, N'6', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (592, 89, N'6A', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (593, 89, N'6AN', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (594, 89, N'6AS', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (595, 89, N'6B', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (596, 89, N'7', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (597, 89, N'7A', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (598, 89, N'7B', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (599, 89, N'7C', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (600, 89, N'7D', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (601, 89, N'7E', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (602, 89, N'7F', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (603, 89, N'7G', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (604, 89, N'7H', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (605, 89, N'7J', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (606, 89, N'7K', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (607, 89, N'8A', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (608, 89, N'8B', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (609, 89, N'8D', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (610, 90, N'8C', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (611, 90, N'9A', N'KLM', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (612, 91, N'2A', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (613, 91, N'3A', N'HMK', NULL, 3)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (614, 91, N'3A', N'HMK', NULL, 4)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (615, 91, N'4A', N'HMK', NULL, 3)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (616, 91, N'4A', N'HMK', NULL, 4)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (617, 91, N'5B', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (618, 91, N'5B1', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (619, 91, N'5B2', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (620, 91, N'6A', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (621, 91, N'6AN', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (622, 91, N'6AS', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (623, 91, N'7A', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (624, 91, N'7B', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (625, 91, N'7C', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (626, 91, N'7E', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (627, 91, N'7F', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (628, 91, N'7G', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (629, 91, N'7H', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (630, 91, N'7J', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (631, 91, N'7K', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (632, 91, N'8', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (633, 91, N'8A', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (634, 91, N'8B', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (635, 91, N'8C', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (636, 91, N'8D', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (637, 91, N'8E', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (638, 92, N'3A', N'HMK', NULL, 1)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (639, 92, N'3A', N'HMK', NULL, 2)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (640, 92, N'20', N'HMK', NULL, 1)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (641, 92, N'20', N'HMK', NULL, 2)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (642, 92, N'21', N'HMK', NULL, 1)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (643, 92, N'21', N'HMK', NULL, 2)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (644, 92, N'4B', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (645, 92, N'4C', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (646, 92, N'7D', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (647, 93, N'9A', N'HMK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (648, 99, N'3A', N'RTG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (649, 99, N'20', N'RTG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (650, 99, N'21', N'RTG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (651, 99, N'4', N'RTG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (652, 99, N'4A', N'RTG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (653, 99, N'4B', N'RTG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (654, 99, N'4C', N'RTG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (655, 99, N'7D', N'RTG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (656, 102, N'1', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (657, 102, N'2', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (658, 102, N'2A', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (659, 102, N'2B', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (660, 103, N'10', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (661, 103, N'12', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (662, 103, N'14', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (663, 103, N'14B', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (664, 103, N'3A', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (665, 103, N'4', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (666, 103, N'4A', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (667, 103, N'4B', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (668, 103, N'4C', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (669, 103, N'6', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (670, 103, N'6A', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (671, 103, N'6B', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (672, 103, N'7', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (673, 103, N'7A', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (674, 103, N'7B', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (675, 103, N'7C', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (676, 103, N'7D', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (677, 103, N'7E', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (678, 103, N'7F', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (679, 103, N'7G', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (680, 103, N'7H', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (681, 103, N'7J', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (682, 103, N'7K', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (683, 103, N'8', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (684, 103, N'8A', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (685, 103, N'8B', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (686, 103, N'8C', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (687, 103, N'8D', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (688, 103, N'8E', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (689, 103, N'9', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (690, 103, N'9A', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (691, 104, N'5A', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (692, 105, N'5B', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (693, 105, N'5B1', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (694, 105, N'5B2', N'LNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (695, 106, N'1', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (696, 106, N'12', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (697, 106, N'14B', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (698, 106, N'2', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (699, 106, N'2A', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (700, 106, N'2B', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (701, 106, N'3', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (702, 106, N'3A', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (703, 106, N'20', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (704, 106, N'21', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (705, 106, N'3D', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (706, 106, N'4', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (707, 106, N'4A', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (708, 106, N'4B', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (709, 106, N'4C', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (710, 106, N'5', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (711, 106, N'5A', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (712, 106, N'5B', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (713, 106, N'5B1', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (714, 106, N'5B2', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (715, 106, N'6', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (716, 106, N'6A', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (717, 106, N'6AN', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (718, 106, N'6AS', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (719, 106, N'6B', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (720, 106, N'7', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (721, 106, N'7A', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (722, 106, N'7B', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (723, 106, N'7C', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (724, 106, N'7D', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (725, 106, N'7E', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (726, 106, N'7F', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (727, 106, N'7G', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (728, 106, N'7H', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (729, 106, N'7J', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (730, 106, N'7K', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (731, 106, N'8', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (732, 106, N'8A', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (733, 106, N'8B', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (734, 106, N'8C', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (735, 106, N'8D', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (736, 106, N'8E', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (737, 106, N'9', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (738, 106, N'9A', N'MAK', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (739, 107, N'7B', N'GHV', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (740, 107, N'7C', N'GHV', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (741, 107, N'7D', N'GHV', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (742, 107, N'7E', N'GHV', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (743, 107, N'7F', N'GHV', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (744, 107, N'7G', N'GHV', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (745, 107, N'7H', N'GHV', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (746, 107, N'7J', N'GHV', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (747, 107, N'7K', N'GHV', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (748, 107, N'8A', N'GHV', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (749, 107, N'8B', N'GHV', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (750, 107, N'8D', N'GHV', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (751, 108, N'8C', N'GHV', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (752, 108, N'9A', N'GHV', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (753, 109, N'7', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (754, 109, N'7A', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (755, 109, N'7B', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (756, 109, N'7C', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (757, 109, N'7D', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (758, 109, N'7E', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (759, 109, N'7F', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (760, 109, N'7G', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (761, 109, N'7H', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (762, 109, N'7J', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (763, 109, N'7K', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (764, 109, N'8A', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (765, 109, N'8B', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (766, 110, N'7', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (767, 110, N'7A', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (768, 110, N'7B', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (769, 110, N'7C', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (770, 110, N'7D', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (771, 110, N'7E', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (772, 110, N'7F', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (773, 110, N'7G', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (774, 110, N'7H', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (775, 110, N'7J', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (776, 110, N'7K', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (777, 110, N'8A', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (778, 110, N'8B', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (779, 110, N'8D', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (780, 111, N'8C', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (781, 111, N'9A', N'HAT', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (782, 137, N'3A', N'DVH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (783, 137, N'20', N'DVH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (784, 137, N'21', N'DVH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (785, 143, N'3A', N'SPE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (786, 143, N'20', N'SPE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (787, 143, N'21', N'SPE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (788, 143, N'4', N'SPE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (789, 143, N'4A', N'SPE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (790, 143, N'4B', N'SPE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (791, 143, N'4C', N'SPE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (792, 144, N'6A', N'SPE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (793, 144, N'6AN', N'SPE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (794, 144, N'6AS', N'SPE', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (795, 154, N'7', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (796, 154, N'7A', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (797, 154, N'7B', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (798, 154, N'7C', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (799, 154, N'7D', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (800, 154, N'7E', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (801, 154, N'7F', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (802, 154, N'7G', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (803, 154, N'7H', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (804, 154, N'7J', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (805, 154, N'7K', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (806, 155, N'8A', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (807, 155, N'8B', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (808, 155, N'8D', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (809, 156, N'8C', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (810, 156, N'9A', N'PIL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (811, 158, N'21', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (812, 158, N'23', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (813, 158, N'22', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (814, 159, N'3D', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (815, 159, N'24', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (816, 159, N'25', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (817, 159, N'26', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (818, 159, N'27', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (819, 159, N'28', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (820, 159, N'29', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (821, 159, N'30', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (822, 159, N'31', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (823, 159, N'32', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (824, 160, N'20', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (825, 160, N'4', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (826, 160, N'4A', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (827, 160, N'4B', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (828, 160, N'4C', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (829, 161, N'7A', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (830, 162, N'7B', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (831, 162, N'7C', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (832, 163, N'7D', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (833, 164, N'7E', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (834, 165, N'7F', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (835, 165, N'7G', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (836, 166, N'7H', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (837, 166, N'7J', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (838, 166, N'7K', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (839, 167, N'8', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (840, 167, N'8A', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (841, 167, N'8B', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (842, 167, N'8C', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (843, 167, N'8D', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (844, 167, N'8E', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (845, 167, N'9A', N'RSP', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (846, 168, N'1', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (847, 168, N'2', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (848, 168, N'2A', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (849, 168, N'2B', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (850, 169, N'20', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (851, 169, N'21', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (852, 169, N'4', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (853, 169, N'4A', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (854, 169, N'4B', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (855, 169, N'4C', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (856, 169, N'6', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (857, 169, N'6A', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (858, 169, N'6AN', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (859, 169, N'6B', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (860, 170, N'5A', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (861, 171, N'5B', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (862, 171, N'5B1', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (863, 171, N'5B2', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (864, 172, N'10', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (865, 172, N'7', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (866, 172, N'7A', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (867, 172, N'7B', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (868, 172, N'7C', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (869, 172, N'7D', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (870, 172, N'7E', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (871, 172, N'7F', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (872, 172, N'7G', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (873, 172, N'7H', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (874, 172, N'7J', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (875, 172, N'7K', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (876, 172, N'8', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (877, 172, N'8A', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (878, 172, N'8B', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (879, 172, N'8C', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (880, 172, N'8D', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (881, 172, N'8E', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (882, 172, N'9', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (883, 172, N'9A', N'MSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (884, 173, N'3A', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (885, 173, N'20', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (886, 173, N'21', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (887, 173, N'4', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (888, 173, N'4A', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (889, 173, N'4B', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (890, 173, N'4C', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (891, 174, N'6', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (892, 174, N'6A', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (893, 174, N'6AN', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (894, 174, N'6AS', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (895, 174, N'6B', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (896, 174, N'7', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (897, 174, N'7A', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (898, 174, N'7B', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (899, 174, N'7C', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (900, 174, N'7D', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (901, 174, N'7E', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (902, 174, N'7F', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (903, 174, N'7G', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (904, 174, N'7H', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (905, 174, N'7J', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (906, 174, N'7K', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (907, 175, N'8', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (908, 175, N'8A', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (909, 175, N'8B', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (910, 175, N'8C', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (911, 175, N'8D', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (912, 175, N'8E', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (913, 175, N'9A', N'LSJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (914, 176, N'1', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (915, 176, N'10', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (916, 176, N'12', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (917, 176, N'14', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (918, 176, N'14B', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (919, 176, N'2', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (920, 176, N'2A', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (921, 176, N'2B', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (922, 176, N'3', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (923, 176, N'3A', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (924, 176, N'20', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (925, 176, N'21', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (926, 176, N'23', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (927, 176, N'22', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (928, 176, N'3D', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (929, 176, N'24', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (930, 176, N'25', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (931, 176, N'26', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (932, 176, N'27', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (933, 176, N'28', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (934, 176, N'29', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (935, 176, N'30', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (936, 176, N'31', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (937, 176, N'32', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (938, 176, N'4', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (939, 176, N'4A', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (940, 176, N'4B', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (941, 176, N'4C', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (942, 176, N'5', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (943, 176, N'5A', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (944, 176, N'5B', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (945, 176, N'5B1', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (946, 176, N'5B2', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (947, 176, N'6', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (948, 176, N'6A', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (949, 176, N'6B', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (950, 176, N'7', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (951, 176, N'7A', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (952, 176, N'7B', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (953, 176, N'7C', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (954, 176, N'7D', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (955, 176, N'7E', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (956, 176, N'7F', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (957, 176, N'7G', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (958, 176, N'7H', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (959, 176, N'7J', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (960, 176, N'7K', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (961, 176, N'8', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (962, 176, N'8A', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (963, 176, N'8B', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (964, 176, N'8C', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (965, 176, N'8D', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (966, 176, N'8E', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (967, 176, N'9', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (968, 176, N'9A', N'SHJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (969, 177, N'1', N'DRJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (970, 177, N'2', N'DRJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (971, 177, N'2A', N'DRJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (972, 177, N'2B', N'DRJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (973, 178, N'20', N'DRJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (974, 178, N'21', N'DRJ', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (975, 180, N'10', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (976, 180, N'12', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (977, 181, N'3A', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (978, 181, N'20', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (979, 181, N'21', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (980, 181, N'4', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (981, 181, N'4A', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (982, 181, N'4B', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (983, 181, N'4C', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (984, 181, N'7D', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (985, 182, N'6', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (986, 182, N'6A', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (987, 182, N'6AN', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (988, 182, N'6AS', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (989, 182, N'6B', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (990, 182, N'7A', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (991, 182, N'7B', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (992, 182, N'7C', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (993, 182, N'7E', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (994, 182, N'7F', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (995, 182, N'7G', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (996, 182, N'7H', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (997, 182, N'7J', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (998, 182, N'7K', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (999, 183, N'8', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1000, 183, N'8A', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1001, 183, N'8B', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1002, 183, N'8C', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1003, 183, N'8D', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1004, 183, N'8E', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1005, 183, N'9A', N'HRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1006, 189, N'1', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1007, 189, N'2', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1008, 189, N'2A', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1009, 189, N'2B', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1010, 190, N'12', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1011, 190, N'14', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1012, 190, N'14B', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1013, 190, N'5', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1014, 190, N'5A', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1015, 190, N'5B', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1016, 190, N'5B1', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1017, 190, N'5B2', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1018, 190, N'6', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1019, 190, N'6A', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1020, 190, N'6AN', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1021, 190, N'6AS', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1022, 190, N'6B', N'RDF', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1023, 193, N'3A', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1024, 193, N'20', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1025, 193, N'21', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1026, 193, N'4', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1027, 193, N'4A', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1028, 193, N'4B', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1029, 193, N'4C', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1030, 194, N'6', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1031, 194, N'6A', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1032, 194, N'6AN', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1033, 194, N'6AS', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1034, 194, N'6B', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1035, 194, N'7A', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1036, 194, N'7B', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1037, 194, N'7C', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1038, 194, N'7E', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1039, 194, N'7F', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1040, 194, N'7G', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1041, 194, N'7H', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1042, 195, N'8', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1043, 195, N'8A', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1044, 195, N'8B', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1045, 195, N'8C', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1046, 195, N'8D', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1047, 195, N'8E', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1048, 195, N'9A', N'SKD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1049, 196, N'3A', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1050, 196, N'20', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1051, 196, N'21', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1052, 196, N'4', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1053, 196, N'4A', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1054, 196, N'4B', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1055, 196, N'4C', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1056, 196, N'7D', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1057, 197, N'6', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1058, 197, N'6A', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1059, 197, N'6AN', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1060, 197, N'6AS', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1061, 197, N'6B', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1062, 198, N'7A', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1063, 198, N'7F', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1064, 198, N'7G', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1065, 199, N'7E', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1066, 200, N'8', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1067, 200, N'8A', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1068, 200, N'8B', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1069, 200, N'8C', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1070, 200, N'8D', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1071, 200, N'8E', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1072, 201, N'9A', N'SMR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1073, 204, N'6', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1074, 204, N'6A', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1075, 204, N'6AN', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1076, 204, N'6AS', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1077, 204, N'6B', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1078, 204, N'7', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1079, 204, N'7A', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1080, 204, N'7B', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1081, 204, N'7C', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1082, 204, N'7D', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1083, 204, N'7E', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1084, 204, N'7F', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1085, 204, N'7G', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1086, 204, N'7H', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1087, 204, N'7J', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1088, 204, N'7K', N'GGR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1089, 205, N'4A', N'BBR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1090, 205, N'6', N'BBR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1091, 205, N'6A', N'BBR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1092, 205, N'6AN', N'BBR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1093, 205, N'6AS', N'BBR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1094, 205, N'6B', N'BBR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1095, 206, N'4C', N'BBR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1096, 206, N'7D', N'BBR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1097, 207, N'7A', N'BBR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1098, 207, N'7F', N'BBR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1099, 207, N'7G', N'BBR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1100, 208, N'7E', N'BBR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1101, 209, N'9A', N'BBR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1102, 210, N'6', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1103, 210, N'6A', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1104, 210, N'6AN', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1105, 210, N'6AS', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1106, 210, N'6B', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1107, 210, N'7', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1108, 210, N'7A', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1109, 210, N'7B', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1110, 210, N'7C', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1111, 210, N'7D', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1112, 210, N'7E', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1113, 210, N'7F', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1114, 210, N'7G', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1115, 210, N'7H', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1116, 210, N'7J', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1117, 210, N'7K', N'SAR', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1118, 211, N'3A', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1119, 211, N'20', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1120, 211, N'21', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1121, 211, N'4', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1122, 211, N'4A', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1123, 211, N'4B', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1124, 211, N'4C', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1125, 211, N'7D', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1126, 212, N'6', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1127, 212, N'6A', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1128, 212, N'6B', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1129, 212, N'7B', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1130, 212, N'7J', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1131, 213, N'7A', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1132, 213, N'7E', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1133, 213, N'7F', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1134, 213, N'7G', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1135, 213, N'7H', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1136, 214, N'8', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1137, 214, N'8A', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1138, 214, N'8B', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1139, 214, N'8C', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1140, 214, N'8D', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1141, 214, N'8E', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1142, 215, N'9A', N'SRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1143, 216, N'3A', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1144, 216, N'20', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1145, 216, N'21', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1146, 216, N'4', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1147, 216, N'4A', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1148, 216, N'4B', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1149, 216, N'4C', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1150, 217, N'6', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1151, 217, N'6A', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1152, 217, N'6B', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1153, 217, N'7', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1154, 217, N'7A', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1155, 217, N'7B', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1156, 217, N'7C', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1157, 217, N'7D', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1158, 217, N'7E', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1159, 217, N'7F', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1160, 217, N'7G', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1161, 217, N'7H', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1162, 217, N'7J', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1163, 217, N'7K', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1164, 217, N'8A', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1165, 217, N'8B', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1166, 217, N'8D', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1167, 218, N'8C', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1168, 219, N'9A', N'PRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1169, 220, N'1', N'TRB', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1170, 220, N'2A', N'TRB', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1171, 220, N'2B', N'TRB', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1172, 220, N'3A', N'TRB', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1173, 220, N'20', N'TRB', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1174, 220, N'21', N'TRB', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1175, 220, N'4', N'TRB', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1176, 220, N'4A', N'TRB', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1177, 220, N'4B', N'TRB', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1178, 220, N'4C', N'TRB', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1179, 226, N'1', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1180, 226, N'2', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1181, 226, N'2A', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1182, 226, N'2B', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1183, 226, N'4', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1184, 226, N'4A', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1185, 226, N'4B', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1186, 226, N'4C', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1187, 226, N'8', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1188, 226, N'8A', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1189, 226, N'8B', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1190, 226, N'8C', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1191, 226, N'8D', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1192, 226, N'8E', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1193, 226, N'9', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1194, 226, N'9A', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1195, 227, N'3A', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1196, 227, N'20', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1197, 227, N'21', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1198, 229, N'5B', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1199, 229, N'5B1', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1200, 229, N'5B2', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1201, 229, N'6', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1202, 229, N'6A', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1203, 229, N'6B', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1204, 229, N'7', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1205, 229, N'7A', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1206, 229, N'7B', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1207, 229, N'7C', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1208, 229, N'7D', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1209, 229, N'7E', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1210, 229, N'7F', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1211, 229, N'7G', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1212, 229, N'7H', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1213, 229, N'7J', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1214, 229, N'7K', N'SKO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1215, 231, N'23', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1216, 231, N'22', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1217, 231, N'24', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1218, 231, N'25', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1219, 231, N'26', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1220, 231, N'27', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1221, 231, N'28', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1222, 231, N'29', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1223, 231, N'30', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1224, 231, N'31', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1225, 232, N'32', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1226, 234, N'1', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1227, 234, N'10', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1228, 234, N'12', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1229, 234, N'2', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1230, 234, N'3A', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1231, 234, N'4', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1232, 234, N'4A', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1233, 234, N'4B', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1234, 234, N'4C', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1235, 234, N'5', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1236, 234, N'5A', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1237, 234, N'5B', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1238, 234, N'6', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1239, 234, N'6A', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1240, 234, N'6B', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1241, 234, N'7', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1242, 234, N'7A', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1243, 234, N'7B', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1244, 234, N'7C', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1245, 234, N'7D', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1246, 234, N'7E', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1247, 234, N'7F', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1248, 234, N'7G', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1249, 234, N'7H', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1250, 234, N'7J', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1251, 234, N'7K', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1252, 234, N'8', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1253, 234, N'8A', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1254, 234, N'8B', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1255, 234, N'8C', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1256, 234, N'8D', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1257, 234, N'8E', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1258, 234, N'9', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1259, 234, N'9A', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1260, 235, N'14', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1261, 235, N'14B', N'LKS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1262, 251, N'6', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1263, 251, N'6A', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1264, 251, N'6B', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1265, 251, N'7', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1266, 251, N'7A', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1267, 251, N'7B', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1268, 251, N'7C', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1269, 251, N'7D', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1270, 251, N'7E', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1271, 251, N'7F', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1272, 251, N'7G', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1273, 251, N'7H', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1274, 251, N'7J', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1275, 251, N'7K', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1276, 252, N'8', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1277, 252, N'8A', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1278, 252, N'8B', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1279, 252, N'8C', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1280, 252, N'8D', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1281, 252, N'8E', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1282, 252, N'9A', N'RIH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1283, 253, N'3A', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1284, 253, N'20', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1285, 253, N'21', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1286, 253, N'23', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1287, 253, N'22', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1288, 253, N'24', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1289, 254, N'4', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1290, 254, N'4A', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1291, 254, N'4B', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1292, 254, N'4C', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1293, 255, N'7A', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1294, 256, N'7B', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1295, 256, N'7C', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1296, 257, N'7D', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1297, 258, N'7E', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1298, 259, N'7F', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1299, 259, N'7G', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1300, 260, N'7H', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1301, 260, N'7J', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1302, 260, N'7K', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1303, 261, N'8A', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1304, 261, N'8B', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1305, 262, N'8C', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1306, 262, N'9A', N'TNG', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1307, 263, N'23', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1308, 263, N'22', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1309, 263, N'3D', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1310, 263, N'24', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1311, 263, N'25', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1312, 263, N'26', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1313, 263, N'27', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1314, 263, N'28', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1315, 263, N'29', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1316, 263, N'30', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1317, 263, N'31', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1318, 263, N'32', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1319, 264, N'3A', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1320, 264, N'20', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1321, 264, N'21', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1322, 264, N'4', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1323, 264, N'4A', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1324, 264, N'4B', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1325, 264, N'4C', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1326, 265, N'6', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1327, 265, N'6A', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1328, 265, N'6B', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1329, 265, N'7A', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1330, 265, N'7B', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1331, 265, N'7C', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1332, 265, N'7F', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1333, 265, N'7G', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1334, 265, N'7H', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1335, 265, N'7J', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1336, 265, N'7K', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1337, 266, N'7D', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1338, 266, N'7E', N'BRS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1339, 274, N'3A', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1340, 274, N'20', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1341, 274, N'21', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1342, 274, N'4', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1343, 274, N'4A', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1344, 274, N'4B', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1345, 274, N'4C', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1346, 274, N'7D', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1347, 275, N'6', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1348, 275, N'6A', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1349, 275, N'6AN', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1350, 275, N'6AS', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1351, 275, N'6B', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1352, 275, N'7A', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1353, 275, N'7B', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1354, 275, N'7C', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1355, 275, N'7E', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1356, 275, N'7F', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1357, 275, N'7G', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1358, 275, N'7H', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1359, 275, N'7J', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1360, 276, N'8A', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1361, 276, N'8B', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1362, 276, N'8D', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1363, 277, N'8C', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1364, 277, N'9A', N'SRH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1365, 278, N'6', N'RHS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1366, 278, N'6A', N'RHS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1367, 278, N'6B', N'RHS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1368, 278, N'7', N'RHS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1369, 278, N'7A', N'RHS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1370, 278, N'7B', N'RHS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1371, 278, N'7C', N'RHS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1372, 278, N'7D', N'RHS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1373, 278, N'7E', N'RHS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1374, 278, N'7F', N'RHS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1375, 278, N'7G', N'RHS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1376, 278, N'7H', N'RHS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1377, 278, N'7J', N'RHS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1378, 278, N'7K', N'RHS', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1379, 281, N'23', N'ORD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1380, 281, N'22', N'ORD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1381, 281, N'3D', N'ORD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1382, 281, N'24', N'ORD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1383, 281, N'25', N'ORD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1384, 281, N'26', N'ORD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1385, 281, N'27', N'ORD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1386, 281, N'28', N'ORD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1387, 281, N'29', N'ORD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1388, 281, N'30', N'ORD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1389, 281, N'31', N'ORD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1390, 281, N'32', N'ORD', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1391, 283, N'23', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1392, 283, N'22', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1393, 283, N'3D', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1394, 283, N'24', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1395, 283, N'25', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1396, 283, N'26', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1397, 283, N'27', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1398, 283, N'28', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1399, 283, N'29', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1400, 283, N'30', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1401, 283, N'31', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1402, 283, N'32', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1403, 284, N'3A', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1404, 284, N'20', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1405, 284, N'21', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1406, 285, N'4', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1407, 285, N'4A', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1408, 285, N'4B', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1409, 285, N'4C', N'PGH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1410, 286, N'1', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1411, 286, N'2', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1412, 286, N'2A', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1413, 286, N'2B', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1414, 288, N'3A', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1415, 288, N'20', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1416, 288, N'21', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1417, 288, N'4', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1418, 288, N'4A', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1419, 288, N'4B', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1420, 288, N'4C', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1421, 288, N'5B', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1422, 288, N'5B1', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1423, 288, N'5B2', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1424, 288, N'6A', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1425, 288, N'7', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1426, 288, N'7A', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1427, 288, N'7B', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1428, 288, N'7C', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1429, 288, N'7D', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1430, 288, N'7E', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1431, 288, N'7F', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1432, 288, N'7G', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1433, 288, N'7H', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1434, 288, N'7J', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1435, 288, N'7K', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1436, 288, N'8', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1437, 288, N'8A', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1438, 288, N'8B', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1439, 288, N'8C', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1440, 288, N'8D', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1441, 288, N'8E', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1442, 288, N'9', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1443, 288, N'9A', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1444, 289, N'14', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1445, 289, N'14B', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1446, 289, N'5A', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1447, 290, N'6B', N'BRO', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1448, 291, N'1', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1449, 291, N'12', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1450, 291, N'14', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1451, 291, N'14B', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1452, 291, N'2', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1453, 291, N'2A', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1454, 291, N'2B', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1455, 291, N'3', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1456, 291, N'3A', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1457, 291, N'20', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1458, 291, N'21', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1459, 291, N'23', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1460, 291, N'22', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1461, 291, N'3D', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1462, 291, N'24', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1463, 291, N'25', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1464, 291, N'26', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1465, 291, N'27', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1466, 291, N'28', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1467, 291, N'29', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1468, 291, N'30', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1469, 291, N'31', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1470, 291, N'32', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1471, 291, N'4', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1472, 291, N'4A', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1473, 291, N'4B', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1474, 291, N'4C', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1475, 291, N'5', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1476, 291, N'5A', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1477, 291, N'5B', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1478, 291, N'5B1', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1479, 291, N'5B2', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1480, 291, N'6', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1481, 291, N'6A', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1482, 291, N'6AN', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1483, 291, N'6AS', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1484, 291, N'6B', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1485, 291, N'7', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1486, 291, N'7A', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1487, 291, N'7B', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1488, 291, N'7C', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1489, 291, N'7D', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1490, 291, N'7E', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1491, 291, N'7F', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1492, 291, N'7G', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1493, 291, N'7H', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1494, 291, N'7J', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1495, 291, N'7K', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1496, 291, N'8', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1497, 291, N'8A', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1498, 291, N'8B', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1499, 291, N'8C', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1500, 291, N'8D', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1501, 291, N'8E', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1502, 291, N'9', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1503, 291, N'9A', N'BLH', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1504, 292, N'3A', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1505, 292, N'20', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1506, 292, N'21', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1507, 293, N'4', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1508, 293, N'4A', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1509, 293, N'4B', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1510, 293, N'4C', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1511, 293, N'7D', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1512, 294, N'6A', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1513, 294, N'6AN', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1514, 294, N'6AS', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1515, 295, N'6B', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1516, 296, N'7A', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1517, 297, N'7B', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1518, 297, N'7C', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1519, 297, N'7E', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1520, 297, N'7F', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1521, 297, N'7G', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1522, 297, N'7H', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1523, 297, N'7J', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1524, 297, N'7K', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1525, 298, N'8', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1526, 298, N'8A', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1527, 298, N'8B', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1528, 298, N'8C', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1529, 298, N'8D', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1530, 298, N'8E', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1531, 298, N'9A', N'HVL', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1532, 299, N'3A', N'SKI', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1533, 299, N'20', N'SKI', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1534, 299, N'21', N'SKI', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1535, 299, N'4', N'SKI', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1536, 299, N'4A', N'SKI', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1537, 299, N'4B', N'SKI', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1538, 299, N'4C', N'SKI', NULL, NULL)
GO
INSERT [dbo].[R_StockSpeciesArea] ([r_StockSpeciesAreaId], [L_stockId], [DFUArea], [speciesCode], [statisticalRectangle], [quarter]) VALUES (1539, 299, N'7D', N'SKI', NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[R_StockSpeciesArea] OFF
GO










USE [FishLineSecurity]
GO


--Add security tasks
INSERT INTO FishLineTasks
VALUES ('AddEditSDEventsAndSamples'),
       ('ViewSDEventsAndSamples'),
	   ('EditSDAnnotations'),
	   ('DeleteAnimals')
GO


DECLARE @AddEditSDEventsAndSamplesId int
DECLARE @ViewSDEventsAndSamplesId int
DECLARE @EditSDAnnotationsId int
DECLARE @DeleteAnimalsId int

SET @AddEditSDEventsAndSamplesId = (SELECT FishLineTaskId FROM FishLineTasks WHERE Value = 'AddEditSDEventsAndSamples' )
SET @ViewSDEventsAndSamplesId = (SELECT FishLineTaskId FROM FishLineTasks WHERE Value = 'ViewSDEventsAndSamples' )
SET @EditSDAnnotationsId = (SELECT FishLineTaskId FROM FishLineTasks WHERE Value = 'EditSDAnnotations' )
SET @DeleteAnimalsId = (SELECT FishLineTaskId FROM FishLineTasks WHERE Value = 'DeleteAnimals' )


-- Add new tasks to admin role
INSERT INTO FishLineTaskRole  
VALUES (@AddEditSDEventsAndSamplesId, 4),
       (@ViewSDEventsAndSamplesId, 4),
       (@EditSDAnnotationsId, 4),
	   (@DeleteAnimalsId, 4)
GO




USE [FishLineDW]
GO

-- Add sdAgeInfoUpdated column.
ALTER TABLE [dbo].[Age]
ADD [sdAgeInfoUpdated] bit NULL
GO

-- Add sdAgeReadId column.
ALTER TABLE [dbo].[Age]
ADD [sdAgeReadId] int NULL
GO

-- Add sdAgeReadName column.
ALTER TABLE [dbo].[Age]
ADD [sdAgeReadName] [nvarchar](80) NULL
GO

-- Add sdAnnotationId column.
ALTER TABLE [dbo].[Age]
ADD [sdAnnotationId] int  NULL
GO