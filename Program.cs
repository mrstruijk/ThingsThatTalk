using System;
using StereoKit;
using StereoKit.Framework;


namespace ProjectName
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var passthrough = SK.GetOrCreateStepper<PassthroughFBExt>();

            var settings = new SKSettings
            {
                appName = "ProjectName",
                assetsFolder = "Assets",
                blendPreference = DisplayBlend.Additive
            };

            if (!SK.Initialize(settings))
            {
                return;
            }

            var modelScalerPose = new Pose(0.5f, 0.2f, -0.1f, Quat.LookDir(0, 0, 1));
            var modelTogglerPose = new Pose(0.5f, 0, -0.1f, Quat.LookDir(0, 0, 1));
            var modelPose = new Pose(0f, 0f, -0.5f, Quat.LookDir(0, 0, 1));
            var itemDescriptionPose = new Pose(-.2f, 0f, -0.5f, Quat.LookDir(0, 0, 1));
            var ptTogglePose = new Pose(-0.5f, 0f, -0.1f, Quat.LookDir(0, 0, 1));

            SetSkyBox();

            var models = new[]
            {
                Model.FromFile("sax.glb"),
                Model.FromFile("DamagedHelmet.gltf"),
                Model.FromFile("zun.glb"),
                Model.FromFile("Keyboard_test.glb"),
                Model.FromFile("horn.glb"),
                Model.FromFile("God_Visor.glb")
            };

            var scales = new[]
            {
                1f,
                0.1f,
                1f,
                1f,
                1f,
                1f
            };

            var titles = new[]
            {
                "Charlie Parker's Saxophone",
                "PTP bio-helmet, ca. 2674 AD",
                "Taotie zun (ceremonial vessel), ca. 1100 BCE",
                "Vesuvian-era Keyboard",
                "Hunting Horn, ca. 1494-1500 CE",
                "God Visor, ca. 2021 AD"
            };

            var texts = new[]
            {
                "Alto saxophone owned and played by Charlie Parker. The saxophone consists of seven parts: a body, neck, two [2] mouthpieces, two [2] ligatures and a reed.2019.10.1a: The saxophone body is made of brass and has a sterling silver bell. Some of the keys have mother-of-pearl inlays. The bell is decorated with an engraved floral design with text on the front. The text reads [KING / Super / 20 / THE H.N. WHITE CO. /CLEVELAND – O]. Engraved above the bell lug at the bow of the saxophone is [STERLING / BELL]. Engraved in cursive text below this on the bell lug is [Charlie Parker]. Engraved on the back of the body at the bow, below the key guard is [295173].2019.10.1b: The alto saxophone neck is made of brass and is covered in cork where the neck attaches to the mouthpiece. Engraved on the neck screw at the bottom of the neck is [295173].",
                "In the desolate aftermath of the first ParaTransPlanetary-biowarfare battles, an enigmatic relic surfaced — a remarkably preserved space helmet. Its sleek visor showcases a hauntingly intact Heads-Up Display, akin to a ghostly imprint on ancient CRT screens. Mysterious ports adorn the back, their purpose eluding discovery, though speculation hint at a connection to liquid cooling systems. Below, intricate tubes raise questions about their function, suggesting a role in sustenance. The artifact's origin, date, creator, and the identity of its final wearer remain shrouded in uncertainty, adding to the mystique of this silent witness to interstellar conflict.",
                "Ceremonial covered vessel, type huo, in the form of an elephant. Wooden stand. Surface: a light green patina. Decoration: cast in low relief; final in the round. Previous custodian or owner: Yamanaka and Co. 山中商会 (1917-1965). Date: ca. 1100 BCE, Late Shang dynasty. Origin: Anyang, probably Henan province, China",
                "Nestled near the historic Vesuvius, a partially melted computer keyboard emerged as an archaeological oddity. Despite the evident fusion, vibrant hues of green and white persist, sparking heated debates on whether the design portrayed leaves or abstract deserts. In a puzzling twist, code breakers, tirelessly striving for centuries, grappled with deciphering the cryptic substitution of the letters W, D, M, S, H, L, K, and J with icons. One theory suggests an ancient cyber-artist's rebellion against the tyranny of traditional keys, opting for an avant-garde language of symbols. Another idea proposes that someone tampered with the keyboard as a futuristic form of digital / physical graffiti.",
                "While the source of the materials and the superb artistry of this hunting horn can be attributed to Sierra Leone, certain of its features suggest that it was made by a Bullom or Temne artist for a European client. The positioning of its mouthpiece, its suspension lugs, as well as the scenes of a European hunt carved in relief and most likely based upon European prints all indicate the horn was produced for export. The heraldic shields and inscriptions carved near its base by this unidentified master artist help narrow the dating of its creation to between 1494 and 1500 and identify it as a royal gift from Crown Prince Manuel I of Portugal to King Ferdinand V of Castile and Aragorn. West African ivories were coveted luxuries in Europe in the late 15th and 16th centuries and considered worthy as royal gifts. Ivory end blown horn with segmented horizontal carvings in tiers from the top to the bottom. The figures are of animals and humans. There are also plant forms (trees) and letters (Greek). Each band is carved with high and low relief objects and separated by lattice, beading or braiding. There are five carved S-shaped ivory loops on the side. Three have metal chains attached.",
                "Unearthed amidst the relics of a technologically enigmatic era, the archaeologists stumbled upon a cutting-edge discovery—an advanced virtual reality headset, now affectionately known as The God Visor. Cloaked in mystery, this futuristic apparatus featured a sleek black design and state-of-the-art features that hinted at a transcendental experience. Its purpose remained speculative, with theories ranging from a sacred tool for immersive worship to a device for unraveling the secrets of the cosmos. The God Visor, a modern marvel of its time, left archaeologists awestruck, grappling with the tantalizing mysteries of the bygone virtual realm."
            };

            int index;
            float slider = 0;

            SK.Run(() =>
            {
                UI.WindowBegin("Model Chooser", ref modelTogglerPose, new Vec2(20, 0) * U.cm);
                UI.HSlider("slider", ref slider, 0, models.Length - 1, 1, 150 * U.mm);
                index = (int) Math.Round(slider);
                UI.WindowEnd();

                if (models[index] != null)
                {
                    UI.Handle("Model Handle", ref modelPose, models[index].Bounds * scales[index]);
                    models[index].Draw(modelPose.ToMatrix(scales[index]));

                    UI.WindowBegin(titles[index], ref itemDescriptionPose, new Vec2(20, 0) * U.cm);
                    UI.Text(texts[index]);
                    UI.WindowEnd();

                    UI.WindowBegin("Sizer", ref modelScalerPose, new Vec2(20, 0) * U.cm);
                    UI.HSlider("scale", ref scales[index], 0.1f, 5f, 0.1f, 150 * U.mm);
                    UI.WindowEnd();
                }

                UI.WindowBegin("Passthrough Toggler", ref ptTogglePose);

                if (UI.Button("Passthrough"))
                {
                    if (passthrough.Enabled)
                    {
                        passthrough.Shutdown();
                    }
                    else
                    {
                        passthrough.Initialize();
                    }
                }

                UI.WindowEnd();
            });
        }


        private static void SetSkyBox()
        {
            var sky = Tex.FromCubemapEquirectangular("kloofendal_28d_misty_4k.hdr");
            Renderer.SkyTex = sky;
            Renderer.SkyLight = sky.CubemapLighting;
        }
    }
}
