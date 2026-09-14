using FilenameBuddy;
using RenderBuddy;
using System.Collections.Generic;

namespace AnimationLib
{
    /// <summary>
    /// This is the class that gets passed around to manage the animation being worked out
    /// </summary>
    public class AnimationsLoader
    {
        #region Properties

        public AnimationContainer Animations { get; private set; }

        public List<Garment> Garments { get; private set; }

        public List<AnimationContainer> AdditionalAnimations { get; set; }

        float Scale { get; set; }

        public Filename ModelFile { get; set; }

        private IRenderer Renderer;

        #endregion //Properties

        #region Methods

        public AnimationsLoader(IRenderer renderer, float scale = 1f)
        {
            Renderer = renderer;
            Scale = scale;
        }

        public void LoadContent()
        {
            Garments = new List<Garment>();
            AdditionalAnimations = new List<AnimationContainer>();

            Animations = new AnimationContainer(Scale);
        }

        private void LoadAnimations(string animationFile)
        {
            var filename = new Filename();
            filename.File = animationFile;

            //read in the animations
            var additionalAnimations = new AnimationContainer(Scale);
            additionalAnimations.ReadSkeletonXml(ModelFile, Renderer);
            additionalAnimations.ReadAnimationXml(filename);

            //add to the main animation container
            Animations.AddAnimations(additionalAnimations);
            AdditionalAnimations.Add(additionalAnimations);
        }

        private Garment LoadGarment(string garmentFile)
        {
            if (!string.IsNullOrEmpty(garmentFile))
            {
                var filename = new Filename();
                filename.File = garmentFile;

                var garment = new Garment(filename, Animations.Skeleton, Renderer);
                garment.AddToSkeleton();
                Garments.Add(garment);

                return garment;
            }

            return null;
        }

        public void Save()
        {
            Animations.WriteXml();
            foreach (var garment in Garments)
            {
                garment.WriteXml();
            }

            foreach (var container in AdditionalAnimations)
            {
                container.WriteAnimationXml();
            }
        }

        public void SaveJson()
        {
            Animations.WriteJson();
            foreach (var garment in Garments)
            {
                garment.WriteJson();
            }

            foreach (var container in AdditionalAnimations)
            {
                container.WriteAnimationJson();
            }
        }

        #endregion //Methods

        #region Loading Methods

        /*
        #region AnimationTest Built-Ins

        public void LoadMai(ContentManager content)
        {
            ModelFile = new Filename();
            Filename animation = new Filename();

            ModelFile.File = @"Content\Mai\MaiModel";
            animation.File = @"Content\Mai\MaiAnimations";

            Animations.ReadSkeletonXml(ModelFile, Renderer, content);
            Animations.ReadAnimationXml(animation, content);

            Filename garmentFile = new Filename();
            garmentFile.File = @"C:\Content\Mai\Clothes\Nekkid.xml";
            var garment = new Garment(garmentFile, Animations.Skeleton, Renderer, content, false);
            garment.AddToSkeleton();
            Garments.Add(garment);
        }

        public void LoadMaiJson(ContentManager content)
        {
            ModelFile = new Filename();
            Filename animation = new Filename();

            ModelFile.File = @"..\..\SampleAnimations\Content\Mai\MaiModel";
            animation.File = @"..\..\SampleAnimations\Content\Mai\MaiAnimations";

            Animations.ReadSkeletonJson(ModelFile, Renderer, content);
            Animations.ReadAnimationJson(animation, content);

            Filename garmentFile = new Filename();
            garmentFile.File = @"..\..\SampleAnimations\Content\Mai\Clothes\Nekkid.json";
            var garment = new Garment(garmentFile, Animations.Skeleton, Renderer, content, true);
            garment.AddToSkeleton();
            Garments.Add(garment);
        }

        public void LoadMai()
        {
            Filename.SetRelativeCurrentDirectory("../../../../../SampleAnimations/");

            ModelFile = new Filename(@"Mai\MaiModel.xml");
            Filename animation = new Filename(@"Mai\MaiAnimations.xml");

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);

            Filename garmentFile = new Filename(@"Mai\Clothes\Nekkid.xml");
            var garment = new Garment(garmentFile, Animations.Skeleton, Renderer, null);
            garment.AddToSkeleton();
            Garments.Add(garment);
        }

        public void LoadMaiJson()
        {
            Filename.SetRelativeCurrentDirectory("../../../../../SampleAnimations/");

            ModelFile = new Filename(@"Mai\MaiModel.json");
            Filename animation = new Filename(@"Mai\MaiAnimations.json");

            Animations.ReadSkeletonJson(ModelFile, Renderer);
            Animations.ReadAnimationJson(animation);

            Filename garmentFile = new Filename(@"Mai\Clothes\Nekkid.json");
            var garment = new Garment(garmentFile, Animations.Skeleton, Renderer, null, true);
            garment.AddToSkeleton();
            Garments.Add(garment);
        }

        #endregion //AnimationTest Built-Ins

        #region Language Warriors

        public void LoadTree()
        {
            Filename.SetCurrentDirectory(@"C:\Projects\languagegame\LanguageWarriors.Content\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();

            ModelFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Tree\tree_model.xml";
            animation.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Tree\tree_animations.xml";

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);
        }

        public void LoadMummy()
        {
            Filename.SetCurrentDirectory(@"C:\Projects\languagegame\LanguageWarriors.Content\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();

            ModelFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Mummy\Mummy_model.xml";
            animation.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Mummy\Mummy_animations.xml";

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);
        }

        public void LoadSkeleton()
        {
            Filename.SetCurrentDirectory(@"C:\Projects\languagegame\LanguageWarriors.Content\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();

            ModelFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Skeleton\Skeleton_model.xml";
            animation.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Skeleton\Skeleton_animations.xml";

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);
        }

        public void LoadDragon()
        {
            Filename.SetCurrentDirectory(@"C:\Projects\languagegame\LanguageWarriors.Content\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();

            ModelFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Dragon\Dragon_model.xml";
            animation.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Dragon\Dragon_animations.xml";

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);
        }

        public void LoadHydra()
        {
            Filename.SetCurrentDirectory(@"C:\Projects\languagegame\LanguageWarriors.Content\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();

            ModelFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Hydra\Hydra_Model.xml";
            animation.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Hydra\Hydra_Animations.xml";

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);
        }

        public void LoadWolf()
        {
            Filename.SetCurrentDirectory(@"C:\Projects\languagegame\LanguageWarriors.Content\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();

            ModelFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Wolf\Wolf_model.xml";
            animation.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Wolf\Wolf_animations.xml";

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);
        }

        public void LoadGoblin()
        {
            Filename.SetCurrentDirectory(@"C:\Projects\languagegame\LanguageWarriors.Content\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();
            Filename garmentFile = new Filename();

            ModelFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Goblin\goblin_model.xml";
            animation.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Goblin\goblin_animations.xml";

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);

            garmentFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\goblin\Grunt\ax.xml";
            var garment = new Garment(garmentFile, Animations.Skeleton, Renderer);
            garment.AddToSkeleton();
            Garments.Add(garment);

            garmentFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\goblin\Grunt\elbow.xml";
            garment = new Garment(garmentFile, Animations.Skeleton, Renderer);
            garment.AddToSkeleton();
            Garments.Add(garment);

            garmentFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\goblin\Grunt\helmet.xml";
            garment = new Garment(garmentFile, Animations.Skeleton, Renderer);
            garment.AddToSkeleton();
            Garments.Add(garment);

            garmentFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\goblin\Grunt\pants.xml";
            garment = new Garment(garmentFile, Animations.Skeleton, Renderer);
            garment.AddToSkeleton();
            Garments.Add(garment);

            garmentFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\goblin\Grunt\shield.xml";
            garment = new Garment(garmentFile, Animations.Skeleton, Renderer);
            garment.AddToSkeleton();
            Garments.Add(garment);
        }

        public void LoadArcher()
        {
            Filename.SetCurrentDirectory(@"C:\Projects\languagegame\LanguageWarriors.Content\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();

            ModelFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Archer\Archer_model.xml";
            animation.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Archer\Archer_animations.xml";

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);
        }

        public void LoadKnight()
        {
            Filename.SetCurrentDirectory(@"C:\Projects\languagegame\LanguageWarriors.Content\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();

            ModelFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Knight\Knight_model.xml";
            animation.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Knight\Knight_animations.xml";

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);
        }

        public void LoadWizard()
        {
            Filename.SetCurrentDirectory(@"C:\Projects\languagegame\LanguageWarriors.Content\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();

            ModelFile.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Wizard\Wizard_model.xml";
            animation.File = @"C:\Projects\languagegame\LanguageWarriors.Content\Content\Monsters\Wizard\Wizard_animations.xml";

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);

            Animations.Skeleton.RootBone.SetPrimaryColor(new Color(55, 155, 240));
        }

        #endregion //Language Warriors

        #region Tassle

        public void LoadTassleCarrie()
        {
            LoadTassle(@"C:\Projects\tasslegame\Windows\Content\Carrie\Carrie model.xml",
                @"C:\Projects\tasslegame\Windows\Content\Carrie\Carrie animations.xml",
                @"C:\Projects\tasslegame\Windows\Content\Carrie\Clothes\nekkid.xml");
        }

        private void LoadTassle(string modelFilename, string animationFilename, string garmentFilename)
        {
            Filename.SetCurrentDirectory(@"C:\Projects\tasslegame\Windows\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();
            Filename garmentFile = new Filename();

            ModelFile.File = modelFilename;
            animation.File = animationFilename;

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);

            garmentFile.File = garmentFilename;
            var garment = new Garment(garmentFile, Animations.Skeleton, Renderer);
            garment.AddToSkeleton();
            Garments.Add(garment);
        }

        #endregion //Tassle

        #region RoboJets

        public void LoadRoboJetValkyrie()
        {
            //LoadRoboJet(@"C:\Projects\RoboJets\RoboJets\RoboJets.SharedProject\Content\Robot\RoboJetModel.xml",
            //	@"C:\Projects\RoboJets\RoboJets\RoboJets.SharedProject\Content\Robot\RoboJetAnimations.xml",
            //	@"C:\Projects\RoboJets\RoboJets\RoboJets.SharedProject\Content\Robot\Clothes\Nekkid.xml");

            LoadRoboJet(@"C:\Projects\RoboJets\RoboJets\RoboJets.Core\Content\Robot\RoboJetModel.xml",
                @"C:\Projects\RoboJets\RoboJets\RoboJets.Core\Content\Robot\RoboJetAnimations.xml");

            var garmentFile = new Filename();

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Parts\Cockpit\Base\Cockpit_Data.xml");
            LoadGarment(garmentFile.File);

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Parts\Arms\Base\Arms_Data.xml");
            LoadGarment(garmentFile.File);

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Parts\Head\Base\Head_Data.xml");
            LoadGarment(garmentFile.File);

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Parts\Launcher\Base\Launcher_Data.xml");
            LoadGarment(garmentFile.File);

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Parts\Legs\Base\Legs_Data.xml");
            LoadGarment(garmentFile.File);

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Parts\Wings\Base\Wings_Data.xml");
            LoadGarment(garmentFile.File);

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Parts\GunStock\Base\GunStock_Data.xml");
            LoadGarment(garmentFile.File);

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Parts\GunBarrel\Base\GunBarrel_Data.xml");
            LoadGarment(garmentFile.File);
        }

        private void LoadRoboJet(string modelFilename, string animationFilename, string garmentFilename = "")
        {
            Filename.SetCurrentDirectory(@"C:\Projects\RoboJets\RoboJets\RoboJets.Core\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();
            Filename garmentFile = new Filename();

            ModelFile.File = modelFilename;
            animation.File = animationFilename;

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);

            if (!string.IsNullOrEmpty(garmentFilename))
            {
                garmentFile.File = garmentFilename;
                var garment = new Garment(garmentFile, Animations.Skeleton, Renderer);
                garment.AddToSkeleton();
                Garments.Add(garment);
            }
        }

        #endregion //RoboJets

        #region Wedding

        public void LoadWeddingTabby()
        {
            LoadWedding(@"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Tabby\TabbyModel.xml",
                @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Tabby\TabbyAnimations.xml",
                @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Tabby\Clothes\Nekkid.xml");

            var weaponFilename = new Filename() { File = @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Tabby\Clothes\LeftWeapon.xml" };
            var garment = new Garment(weaponFilename, Animations.Skeleton, Renderer);
            garment.AddToSkeleton();
            Garments.Add(garment);

            weaponFilename = new Filename() { File = @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Tabby\Clothes\RightWeapon.xml" };
            garment = new Garment(weaponFilename, Animations.Skeleton, Renderer);
            garment.AddToSkeleton();
            Garments.Add(garment);
        }

        public void LoadWeddingDan()
        {
            LoadWedding(@"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Dan\DanModel.xml",
                @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Dan\DanAnimations.xml",
                @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Dan\Clothes\Nekkid.xml");
        }

        public void LoadWeddingCarrie()
        {
            LoadWedding(@"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Carrie\CarrieModel.xml",
                @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Carrie\CarrieAnimations.xml",
                @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Carrie\Clothes\Nekkid.xml");

            var weaponFilename = new Filename() { File = @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Carrie\Clothes\LeftWeapon.xml" };
            var garment = new Garment(weaponFilename, Animations.Skeleton, Renderer);
            garment.AddToSkeleton();
            Garments.Add(garment);

            weaponFilename = new Filename() { File = @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Carrie\Clothes\RightWeapon.xml" };
            garment = new Garment(weaponFilename, Animations.Skeleton, Renderer);
            garment.AddToSkeleton();
            Garments.Add(garment);
        }

        public void LoadWeddingBestMen()
        {
            LoadWedding(@"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\BestMen\BestMenModel.xml",
                @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\BestMen\BestMenAnimations.xml",
                @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\BestMen\Clothes\Nekkid.xml");
        }

        public void LoadWeddingVenue()
        {
            LoadWedding(@"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\WeddingVenue\WeddingVenueModel.xml",
                @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\WeddingVenue\WeddingVenueAnimations.xml");
        }

        public void LoadWeddingDanFireball()
        {
            LoadWedding(@"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Dan\Projectiles\ModelConverted.xml",
                @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Dan\Projectiles\AnimationsConverted.xml");
        }

        public void LoadWeddingCarrieFireball()
        {
            LoadWedding(@"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Carrie\Projectiles\ModelConverted.xml",
                @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Carrie\Projectiles\AnimationsConverted.xml");
        }

        public void LoadWeddingTabbyFireball()
        {
            LoadWedding(@"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Tabby\Projectiles\ModelConverted.xml",
                @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\Tabby\Projectiles\AnimationsConverted.xml");
        }

        public void LoadWeddingBestMenFireball()
        {
            LoadWedding(@"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\BestMen\Projectiles\ModelConverted.xml",
                @"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\BestMen\Projectiles\AnimationsConverted.xml");
        }

        private void LoadWedding(string modelFilename, string animationFilename, string garmentFilename = "")
        {
            Filename.SetCurrentDirectory(@"C:\Projects\weddinggame\WeddingGame.SharedProject\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();
            Filename garmentFile = new Filename();

            ModelFile.File = modelFilename;
            animation.File = animationFilename;

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);

            if (!string.IsNullOrEmpty(garmentFilename))
            {
                garmentFile.File = garmentFilename;
                var garment = new Garment(garmentFile, Animations.Skeleton, Renderer);
                garment.AddToSkeleton();
                Garments.Add(garment);
            }
        }

        #endregion //Wedding

        #region Grimoire

        public void LoadGrimoireCharacter()
        {
            LoadGrimoire(@"Character\Character_Model.xml",
                @"Character\All_Animations.xml");

            LoadAnimations(@"Character\Base_Animations.xml");
            LoadAnimations(@"Character\Grimoire_Base_Animations.xml");

            //LoadGarment(@"Spells\Broom\Broom_Data.xml");
            //LoadAnimations(@"Spells\Broom\Dash_Animations.xml");

            //LoadGarment(@"Spells\Sword\Sword_Data.xml");
            //LoadAnimations(@"Spells\Sword\Slash_Animations.xml");

            //LoadGarment(@"Spells\Shield\Shield_Data.xml");
            //LoadAnimations(@"Spells\Shield\Block_Animations.xml");

            var garmentFile = new Filename();
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Skin\Skin.xml");
            LoadGarment(garmentFile.File);

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Skin\Hands\Hand.xml");
            LoadGarment(garmentFile.File);
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Skin\Feet\Foot.xml");
            LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyes\Iris\Eye.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyes\Eyes2\Eyes.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Nose\Button\ButtonNose.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Ears\Ears2\Ears.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Mouth\Smirk1\Mouth.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyebrows\DefaultEyebrows\Eyebrows.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Hair\LongHair\LongHair.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Tops\LongSleeves\PuffyBlouse\WhitePuffyBlouse.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Hose\Stockings\OrangeStockings.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Shoes\WitchBoots\BlackWitchBoots.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Belt\GoddessBelt\GoddessBelt.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Props\Book\Ouroboros\DarkGreenOuroboros.xml");
            //LoadGarment(garmentFile.File);
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Shoes\PirateBoots\BrownPirateBoots.xml");
            LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Skirt\WideSkirt\BlueWideSkirt.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Hat\WitchHat\BlueWitchHat.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Tops\Jacket\WitchCape\BlueWitchCape.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Shorts\Bloomers\PowderBlueBloomers.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Skirt\WideSkirt\RedWideSkirt.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Hat\WitchHat\RedWitchHat.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Tops\Jacket\WitchCape\RedWitchCape.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Shorts\Bloomers\PinkBloomers.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Full\Mid\SundressAndPetticoat\RedSundressAndPetticoat.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Skirt\SkirtAndPetticoat\RedSkirtAndPetticoat.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Skirt\WideSkirt\PurpleWideSkirt.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Hat\WitchHat\PurpleWitchHat.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Tops\Jacket\WitchCape\PurpleWitchCape.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Shorts\Bloomers\VioletBloomers.xml");
            //LoadGarment(garmentFile.File);

            Animations.SetColor("Skin", new Color(255, 210, 210));
            Animations.SetColor("Lips", Color.HotPink);
            Animations.SetColor("Lashes", new Color(40, 30, 20));
            Animations.SetColor("Eyebrows", new Color(140, 110, 40));
            Animations.SetColor("Eyes", new Color(0, 110, 40));
            Animations.SetColor("EyeShadow", new Color(0, 0, 255));
            //Animations.SetColor("Hair", new Color(255, 255, 100));
            //Animations.SetColor("Hair", new Color(220, 190, 80));
            //Animations.SetColor("Hair", new Color(80,60,40));
            //Animations.SetColor("Hair", new Color(70, 40, 0));
            //Animations.SetColor("Hair", new Color(40, 20, 0));
            Animations.SetColor("Hair", new Color(20, 10, 0));
            Animations.SetColor("FacialHair", new Color(80, 60, 40));
        }

        public void LoadGrimoireLevel()
        {
            LoadGrimoire(@"DemoBoard\Castle model.xml",
                @"DemoBoard\Castle animations.xml",
                string.Empty);
        }

        public void LoadGrimoireWarrior()
        {
            LoadGrimoire(@"Spells\Warrior\Warrior_Model.xml",
                @"Spells\Warrior\Warrior_Animations.xml");
        }

        public void LoadGrimoireMummy()
        {
            LoadGrimoire(@"Spells\Mummy\Mummy_Model.xml",
                @"Spells\Mummy\Mummy_Animations.xml");

            Animations.Skeleton.RootBone.SetPrimaryColor(new Color(64, 64, 255));
            //Animations.Skeleton.RootBone.SetPrimaryColor(new Color(255, 64, 64));
        }

        public void LoadGrimoireWizard()
        {
            LoadGrimoire(@"Spells\Wizard\Wizard_Model.xml",
                @"Spells\Wizard\Wizard_Animations.xml");
        }

        public void LoadGrimoireTree()
        {
            LoadGrimoire(@"Spells\Tree\Tree_Model.xml",
                @"Spells\Tree\Tree_Animations.xml");
        }

        public void LoadGrimoireWolf()
        {
            LoadGrimoire(@"Spells\Wolf\Wolf_Model.xml",
                @"Spells\Wolf\Wolf_Animations.xml");

            //Animations.Skeleton.RootBone.SetPrimaryColor(new Color(64, 64, 255));
            Animations.Skeleton.RootBone.SetPrimaryColor(new Color(255, 64, 64));
        }

        public void LoadGrimoireArcher()
        {
            LoadGrimoire(@"Spells\Archer\Archer_Model.xml",
                @"Spells\Archer\Archer_Animations.xml");
        }

        public void LoadGrimoireDragon()
        {
            LoadGrimoire(@"Spells\Dragon\Dragon_Model.xml",
                @"Spells\Dragon\Dragon_Animations.xml");

            var garmentFile = new Filename();
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Bow.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Parts\Scales\Default\Scales.xml");
            //LoadGarment(garmentFile.File);
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Parts\Scales\Wind\Scales.xml");
            LoadGarment(garmentFile.File);

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Parts\Horns\Wind\Horns.xml");
            LoadGarment(garmentFile.File);

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Parts\Wings\Default\Wings.xml");
            LoadGarment(garmentFile.File);

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Parts\Spines\Wind\Spines.xml");
            LoadGarment(garmentFile.File);

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Parts\Claws\Default\Claws.xml");
            LoadGarment(garmentFile.File);

            Animations.Skeleton.RootBone.SetPrimaryColor(new Color(255, 0, 0));

            //Animations.SetColor("1st", Color.DarkRed);
            //Animations.SetColor("2nd", Color.Yellow);
            //Animations.SetColor("Eyes", Color.Blue);
            //Animations.SetColor("Fireball", Color.Green);

            Animations.SetColor("1st", Color.DarkGreen);
            Animations.SetColor("2nd", Color.Goldenrod);
            Animations.SetColor("Eyes", Color.Blue);
            Animations.SetColor("Fireball", Color.Red);
        }

        public void LoadGrimoireDragonFireball()
        {
            LoadGrimoire(@"Spells\Dragon\Projectiles\Model.xml",
                @"Spells\Dragon\Projectiles\Animations.xml");
        }

        public void LoadGrimoireSkeleton()
        {
            LoadGrimoire(@"Spells\Skeleton\Skeleton_Model.xml",
                @"Spells\Skeleton\Skeleton_Animations.xml");

            Animations.Skeleton.RootBone.SetPrimaryColor(new Color(255, 0, 0));
        }

        public void LoadGrimoireGoblin()
        {
            LoadGrimoire(@"Spells\Goblin\Goblin_Model.xml",
                @"Spells\Goblin\Goblin_Animations.xml");

            var garmentFile = new Filename();
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Grunt\ax.xml");
            LoadGarment(garmentFile.File);
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Grunt\elbow.xml");
            LoadGarment(garmentFile.File);
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Grunt\helmet.xml");
            LoadGarment(garmentFile.File);
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Grunt\pants.xml");
            LoadGarment(garmentFile.File);
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Grunt\shield.xml");
            LoadGarment(garmentFile.File);
        }

        public void LoadGrimoireGoblinAx()
        {
            LoadGrimoire(@"Spells\Goblin\Projectiles\Model.xml",
                @"Spells\Goblin\Projectiles\Animations.xml");
        }

        public void LoadGrimoireArcherArrow()
        {
            LoadGrimoire(@"Spells\Archer\Projectiles\Model.xml",
                @"Spells\Archer\Projectiles\Animations.xml");
        }

        public void LoadGrimoireBroom()
        {
            LoadGrimoire(@"Spells\Broom\Broom_Model.xml",
                @"Spells\Broom\Broom_Animations.xml");
        }

        public void LoadGrimoirePumpkin()
        {
            LoadGrimoire(@"Spells\Pumpkin\Pumpkin_Model.xml",
                @"Spells\Pumpkin\Pumpkin_Animations.xml");
        }

        private void LoadGrimoire(string modelFilename, string animationFilename, string garmentFilename = "")
        {
            Filename.SetRelativeCurrentDirectory("../../../../../Grimoire/Grimoire.Content/");

            ModelFile = new Filename(modelFilename);
            Filename animation = new Filename(animationFilename);

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);
            LoadGarment(garmentFilename);
        }

        #endregion //Grimoire

        #region BeachBlocks

        private void LoadCharacter()
        {
            var garmentFile = new Filename();
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Skin\Skin.xml");
            LoadGarment(garmentFile.File);

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Skin\Hands\Hand.xml");
            LoadGarment(garmentFile.File);
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Skin\Feet\Foot.xml");
            LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyes\Iris\Eye.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyes\Eyes2\Eyes.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Nose\Button\ButtonNose.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Ears\Ears2\Ears.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Mouth\Smirk1\Mouth.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyebrows\DefaultEyebrows\Eyebrows.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Hair\LongHair\LongHair.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Shorts\Bloomers\VioletBloomers.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Skirt\WideSkirt\PurpleWideSkirt.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Tops\LongSleeves\PuffyBlouse\WhitePuffyBlouse.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Hat\WitchHat\PurpleWitchHat.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Hose\Stockings\OrangeStockings.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Shoes\WitchBoots\BlackWitchBoots.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Belt\GoddessBelt\GoddessBelt.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Tops\Jacket\WitchCape\WitchCape.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Props\Book\Ouroboros\DarkGreenOuroboros.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyes\Iris\Eye.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyes\Eyes1\Eyes.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyes\Eyes2\Eyes.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyes\Eyes3\Eyes.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyes\Eyes4\Eyes.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyes\Eyes5\Eyes.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyes\Eyes6\Eyes.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Nose\DefaultNose\Nose.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Nose\Button\ButtonNose.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Nose\Button2\ButtonNose.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Nose\Button3\ButtonNose.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Ears\DefaultEars\Ears.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Ears\Ears2\Ears.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Mouth\BigSmile\BigSmile.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Mouth\Smiling\Smiling.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Mouth\Closed\Closed.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Mouth\Closed2\Mouth.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Mouth\Smirk1\Mouth.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Mouth\Smirk2\Mouth.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Mouth\Kewpie1\Mouth.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Mouth\Kewpie2\Mouth.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Mouth\Kewpie3\Mouth.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Mouth\Thin1\Mouth.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Mouth\Thin2\Mouth.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Mouth\Thin3\Mouth.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\FacialHair\Default\Mustache.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\FacialHair\Default\Soul.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\FacialHair\Default\Goat.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\FacialHair\Default\Riker.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyebrows\DefaultEyebrows\Eyebrows.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyebrows\Eyebrows1\Eyebrows.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyebrows\Eyebrows2\Eyebrows.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Face\Eyebrows\Eyebrows3\Eyebrows.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\SwimwearBottom\Bikini\RedBikiniBottom.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Tops\SwimwearTop\Bikini\RedBikiniTop.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\SwimwearBottom\BowTwoPiece\RedBowTwoPieceBottom.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Tops\SwimwearTop\BowTwoPiece\RedBowTwoPieceTop.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\SwimwearBottom\TightyWhiteys\WhiteTightyWhiteys.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Shorts\BoardShorts\RedBoardShorts.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Shorts\BoardShorts\BlueBoardShorts.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Shorts\BoardShorts\WhiteBoardShorts.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Shorts\BoardShorts\BlackBoardShorts.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Shorts\BoardShorts\GreenBoardShorts.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\SwimwearBottom\Trunks1\RedTrunks1.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\SwimwearBottom\Trunks1\BlueTrunks1.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\SwimwearBottom\Trunks1\WhiteTrunks1.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\SwimwearBottom\Trunks1\BlackTrunks1.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\SwimwearBottom\Trunks1\GreenTrunks1.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Shorts\Bloomers\PinkBloomers.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Shorts\Bloomers\VioletBloomers.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Shorts\Bloomers\WhiteBloomers.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Skirt\WideSkirt\HotPinkWideSkirt.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Skirt\WideSkirt\PurpleWideSkirt.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Skirt\WideSkirt\PinkWideSkirt.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Skirt\WideSkirt\VioletWideSkirt.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Bottoms\Skirt\WideSkirt\WhiteWideSkirt.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Full\Swimsuit\Swimsuit1\GreenSwimsuit1.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Full\Swimsuit\Swimsuit1\BlueSwimsuit1.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Full\Swimsuit\Swimsuit1\BlackSwimsuit1.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Full\Swimsuit\Swimsuit1\RedSwimsuit1.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Full\Swimsuit\Swimsuit1\WhiteSwimsuit1.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Tops\ShortSleeves\Wifebeater\WhiteWifebeater.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Tops\LongSleeves\PuffyBlouse\WhitePuffyBlouse.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Hat\WitchHat\WhiteWitchHat.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Hat\WitchHat\PinkWitchHat.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Hat\WitchHat\HotPinkWitchHat.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Hat\WitchHat\PurpleWitchHat.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Hat\WitchHat\VioletWitchHat.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Hair\LongHair\LongHair.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Hair\PixieCut\PixieCut.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Hair\Timberlake\Timberlake.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Glasses\Cateyes\RedCateyes.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Glasses\Cateyes\WhiteCateyes.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Glasses\Cateyes\WhiteRoseCateyes.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Glasses\Cateyes\BlackCateyes.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Glasses\Aviators\RedAviators.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Glasses\Aviators\WhiteAviators.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Glasses\Aviators\WhiteRoseAviators.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Glasses\Aviators\BlackAviators.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Glasses\Aviators\GoldAviators.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Glasses\Aviators\Gold2Aviators.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Glasses\Rounders\RedRounders.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Glasses\Rounders\WhiteRounders.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Glasses\Rounders\WhiteRoseRounders.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Accessories\Glasses\Rounders\BlackRounders.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Props\Tetrad\TetradS\BlueTetradS.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Props\Tetrad\TetradZ\BlueTetradZ.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Shoes\WitchBoots\WhiteWitchBoots.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Shoes\WitchBoots\BlackWitchBoots.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Shoes\FlipFlops\WhiteFlipFlops.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Shoes\FlipFlops\BlackFlipFlops.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Shoes\FlipFlops\BlueFlipFlops.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Shoes\FlipFlops\GreenFlipFlops.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Shoes\FlipFlops\RedFlipFlops.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Socks\AnkleSocks\WhiteAnkleSocks.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Socks\AnkleSocks\BlackAnkleSocks.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Socks\AnkleSocks\BlueAnkleSocks.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Socks\AnkleSocks\GreenAnkleSocks.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Socks\AnkleSocks\RedAnkleSocks.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Socks\AthleticSocks\WhiteAthleticSocks.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Socks\AthleticSocks\BlackAthleticSocks.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Socks\AthleticSocks\BlueAthleticSocks.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Socks\AthleticSocks\GreenAthleticSocks.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Socks\AthleticSocks\RedAthleticSocks.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Hose\Stockings\WhiteStockings.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Hose\Stockings\BlackStockings.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Hose\Stockings\BlueStockings.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Hose\Stockings\GreenStockings.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Hose\Stockings\PurpleStockings.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Hose\Stockings\VioletStockings.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Hose\Stockings\RedStockings.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Hose\Stockings\OrangeStockings.xml");
            //LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Hose\StripeStockings\PurpleOrangeStripeStockings.xml");
            //LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Hose\StripeStockings\GreenOrangeStripeStockings.xml");
            //LoadGarment(garmentFile.File);

            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Shoes\BunnySlippers\PinkBunnySlippers.xml");
            LoadGarment(garmentFile.File);

            Animations.SetColor("Skin", new Color(255, 210, 210));
            Animations.SetColor("Lips", Color.HotPink);
            Animations.SetColor("Lashes", new Color(40, 30, 20));
            Animations.SetColor("Eyebrows", new Color(140, 110, 40));
            Animations.SetColor("Eyes", new Color(0, 110, 40));
            Animations.SetColor("EyeShadow", new Color(0, 0, 255));
            //Animations.SetColor("Hair", new Color(255, 255, 100));
            //Animations.SetColor("Hair", new Color(220, 190, 80));
            //Animations.SetColor("Hair", new Color(80,60,40));
            //Animations.SetColor("Hair", new Color(70, 40, 0));
            //Animations.SetColor("Hair", new Color(40, 20, 0));
            Animations.SetColor("Hair", new Color(20, 10, 0));
            Animations.SetColor("FacialHair", new Color(80, 60, 40));
        }

        public void LoadBeachBlocksCharacter()
        {
            LoadBeachBlocks(@"C:\Projects\BeachBlocks\BeachBlocks.Core\Content\Character\Character_Model.xml",
                @"C:\Projects\BeachBlocks\BeachBlocks.Core\Content\Character\Base_Animations.xml");

            LoadCharacter();
        }

        private void LoadBeachBlocks(string modelFilename, string animationFilename, string garmentFilename = "")
        {
            Filename.SetCurrentDirectory(@"C:\Projects\BeachBlocks\BeachBlocks.Core\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();

            ModelFile.File = modelFilename;
            animation.File = animationFilename;

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);
            Animations.ReadAnimationXml(new Filename()
            {
                File = @"C:\Projects\BeachBlocks\BeachBlocks.Core\Content\Character\BeachBlocks_Base_Animations.xml"
            });

            if (!string.IsNullOrEmpty(garmentFilename))
            {
                Filename garmentFile = new Filename();
                garmentFile.File = garmentFilename;
                var garment = new Garment(garmentFile, Animations.Skeleton, Renderer);
                garment.AddToSkeleton();
                Garments.Add(garment);
            }
        }

        #endregion //BeachBlocks

        #region Pajamorama

        public void LoadPajamoramaApple()
        {
            LoadPajamorama(@"C:\Projects\PajamoramaMobile\Pajamorama.SharedProject\Content\Character\Character_Model.xml",
                @"C:\Projects\PajamoramaMobile\Pajamorama.SharedProject\Content\Character\Base_Animations.xml");

            var garmentFile = new Filename();
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Skin\Skin.xml");
            LoadGarment(garmentFile.File);
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Skin\Hands\Hand.xml");
            LoadGarment(garmentFile.File);
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Skin\Feet\Foot.xml");
            LoadGarment(garmentFile.File);

            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Shoes\BunnySlippers\PinkBunnySlippers.xml");
            //LoadGarment(garmentFile.File);
            garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Feet\Shoes\PirateBoots\PurplePirateBoots.xml");
            LoadGarment(garmentFile.File);
            //garmentFile.SetFilenameRelativeToPath(ModelFile, @"Clothing\Full\Mid\SundressAndPetticoat\GreenSundressAndPetticoat.xml");
            //LoadGarment(garmentFile.File);

            Animations.SetColor("Skin", new Color(255, 210, 210));
            Animations.SetColor("Lips", Color.HotPink);
            Animations.SetColor("Lashes", new Color(40, 30, 20));
            Animations.SetColor("Eyebrows", new Color(140, 110, 40));
            Animations.SetColor("Eyes", new Color(0, 110, 40));
            Animations.SetColor("EyeShadow", new Color(0, 0, 255));
            Animations.SetColor("Hair", new Color(20, 10, 0));
            Animations.SetColor("FacialHair", new Color(80, 60, 40));
        }
        private void LoadPajamorama(string modelFilename, string animationFilename, string garmentFilename = "")
        {
            Filename.SetCurrentDirectory(@"C:\Projects\PajamoramaMobile\Pajamorama.SharedProject\Content\");
            ModelFile = new Filename();
            Filename animation = new Filename();

            ModelFile.File = modelFilename;
            animation.File = animationFilename;

            Animations.ReadSkeletonXml(ModelFile, Renderer);
            Animations.ReadAnimationXml(animation);
            Animations.ReadAnimationXml(new Filename()
            {
                File = @"C:\Projects\PajamoramaMobile\Pajamorama.SharedProject\Content\Character\BeachBlocks_Base_Animations.xml"
            });

            if (!string.IsNullOrEmpty(garmentFilename))
            {
                Filename garmentFile = new Filename();
                garmentFile.File = garmentFilename;
                var garment = new Garment(garmentFile, Animations.Skeleton, Renderer);
                garment.AddToSkeleton();
                Garments.Add(garment);
            }
        }

        #endregion //Pajamorama
        */
        #endregion Loading Methods
    }
}
