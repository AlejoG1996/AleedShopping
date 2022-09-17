using AleedTiendaShopping.Data.Entities;
using AleedTiendaShopping.Enums;
using AleedTiendaShopping.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AleedTiendaShopping.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;

        public SeedDb(DataContext context, IUserHelper userHelper, IBlobHelper blobHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckCategoriesAsync();
            await CheckRolesAsync();
            await CheckProductsAsync();
            await CheckUserAsync("1017240121", "Alejandro", "Galeano", "galeanomadrigalalejandro@gmail.com", "3054325671", "Calle 33 # 42 53", UserType.Admin, "https://localhost:7116/images/users/1017240121.png");
            await CheckUserAsync("1152708952", "Edwin", "Correa", "edwinvelasquezcorrea@yopmail.com", "3195605554", "Calle 33 # 52 43", UserType.User, "https://localhost:7116/images/users/1152708952.png");

        }

        private async Task<User> CheckUserAsync(
    string document,
    string firstName,
    string lastName,
    string email,
    string phone,
    string address,
    UserType userType,
            string imagepath)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                //cargar imagen al blob
                //Guid imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\users\\{image}", "users");

                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    City = _context.Cities.FirstOrDefault(),
                    UserType = userType,
                    ImageFullPath = imagepath,
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);

            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }


        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Empaques" });
                _context.Categories.Add(new Category { Name = "Bolsas" });
                _context.Categories.Add(new Category { Name = "Regalos" });
                _context.Categories.Add(new Category { Name = "Peluches" });
                _context.Categories.Add(new Category { Name = "Belleza" });
                _context.Categories.Add(new Category { Name = "Accesorios" });
                _context.Categories.Add(new Category { Name = "Flores" });
                _context.Categories.Add(new Category { Name = "Papeles" });
                _context.Categories.Add(new Category { Name = "Globos" });

                _context.Categories.Add(new Category { Name = "Dulces" });
                _context.Categories.Add(new Category { Name = "Bebidas" });
                _context.Categories.Add(new Category { Name = "Decoración" });
               
            }

            await _context.SaveChangesAsync();
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "Colombia",
                    States = new List<State>()
                    {
                        new State()
                        {
                            Name = "Antioquia",
                            Cities = new List<City>() {
                                new City() { Name = "Medellín" },
                                new City() { Name = "Itagüí" },
                                new City() { Name = "Envigado" },
                                new City() { Name = "Bello" },
                                new City() { Name = "Rionegro" },
                                new City() { Name = "Caldas" },
                                new City() { Name = "Sabaneta" },
                                new City() { Name = "Copacabana" },
                            }
                        },

                    }
                });
            }
        }

        private async Task CheckProductsAsync()
        {
            if (!_context.Products.Any())
            {
                await AddProductAsync("Caja 23x23x10cm", "Caja Plegadiza micro corrugada con medidas de 23x23x10 cm ", 6500M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaPlegadiza23x23x10.png" });
                await AddProductAsync("Caja Feliz dia  15.5x8.5x4cm", "Caja Deslizable  marcada con grabado  Feliz dia con medidas de 15.5x8.5x4 cm ", 3700M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaDeslizable15.5x8.5x4cm.png", "https://localhost:7116/images/products/CajaDeslizable15.5x8.5x4cm_1.png" });
                await AddProductAsync("Caja Blanca Feliz dia  15.5x15.5x12cm", "Caja Blanca  marcada con grabado  Feliz dia con medidas de 15.5x15.5x12 cm ", 10300M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaBlanca15.5x15.5x12.png" });
                await AddProductAsync("Caja Micro corrugada Feliz dia  18.5x18.5x9cm", "Caja  micro corrugada  marcada con grabado  Feliz dia con medidas de 18.5x18.5x9 cm ", 6600M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaMicrorrugada18.5x18.5x9.png", "https://localhost:7116/images/products/CajaMicrorrugada18.5x18.5x9_1.png" });
                await AddProductAsync("Caja Blanca Feliz dia  19x19x11.5cm", "Caja Blanca  marcada con grabado  Feliz dia con medidas de 19x19x11.5 cm ", 10900M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaBlanca19x19x11.5.png" });
                await AddProductAsync("Caja Micro corrugada Feliz dia  23x23x10cm", "Caja  micro corrugada  marcada con grabado  Feliz dia con medidas de 23x23x10 cm ", 7600M, 12F, new List<string>() { "Empaques" }, new List<string>() {  "https://localhost:7116/images/products/CajaMicrorrugada23x23x10.png" });
                await AddProductAsync("Caja Deslizable Feliz dia  24x20x6cm", "Caja  blanca deslizable  marcada con grabado  Feliz dia con medidas de 24x20x6 cm ", 7600M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaDeslizable24x20x6.png", "https://localhost:7116/images/products/CajaDeslizable24x20x6_1.png" });
                await AddProductAsync("Caja Micro corrugada Feliz dia  30x26x11cm", "Caja  micro corrugada  marcada con grabado  Feliz dia con medidas de 30x26x11 cm ", 9200M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaMicrorrugada30x26x11.png" });
                await AddProductAsync("Caja Micro corrugada Feliz dia  33x11.5x10cm", "Caja  micro corrugada  marcada con grabado  Feliz dia con medidas de 33x11.5x10 cm ", 9200M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaMicrorrugada33x11.5x10.png", "https://localhost:7116/images/products/CajaMicrorrugada33x11.5x10_1.png" });
                await AddProductAsync("Caja Plegadiza con ventana en acetato  6x6x4cm", "Caja plegadiza con ventana en acetato con medidas de 6x6x4 cm ", 1200M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaPlegadizaAcetato6x6x4.png", "https://localhost:7116/images/products/CajaPlegadizaAcetato6x6x4_1.png" });
                await AddProductAsync("Caja Maletin Acetato 15x6x21cm", "Caja Maletin Acetato  con medidas de 15x6x21 cm ", 3400M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaMaletinAcetato15x6x21.png" });
                await AddProductAsync("Caja Blanca plegadiza  6x6x4cm", "Caja blanca plegadiza con ventana en acetato con medidas de 6x6x4 cm ", 1250M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaPlegadizaAcetatoBl6x6x4.png", "https://localhost:7116/images/products/CajaPlegadizaAcetatoBl6x6x4_1.png" });
                await AddProductAsync("Caja Ecologica deslizable 23x13x2.7cm", "Caja Ecologica deslizable  con medidas de 23x13x2.7 cm ", 3900M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaEcoDeslizable23x13x2.7.png", "https://localhost:7116/images/products/CajaEcoDeslizable23x13x2.7_1.png" });
                await AddProductAsync("Caja Ecologica Acetato 25x25x16cm", "Caja Ecologica deslizable y con ventana en acetato con medidas de 25x25x16 cm ", 5700M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaEcoDeslizableAce25x25x16.png", "https://localhost:7116/images/products/CajaEcoDeslizableAce25x25x16_1.png" });
                await AddProductAsync("Caja Ecologica Acetato 30x30x16cm", "Caja Ecologica deslizable y con ventana en acetato con medidas de  30x30x16 cm ", 5900M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaEcoDeslizableAce30x30x16.png", "https://localhost:7116/images/products/CajaEcoDeslizableAce30x30x16_1.png" });
                await AddProductAsync("Caja Maletin Acetato 20x15x15cm", "Caja Ecologica Maletin Acetato  con medidas de 20x15x15 cm ", 5200M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaMaletinAcetato20x15x15.png" });
                await AddProductAsync("Caja Plegadiza   23x10x35cm", "Caja plegadiza  con medidas de 23x10x35 cm ", 5900M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaPlegadiza23x10x35.png", "https://localhost:7116/images/products/CajaPlegadiza23x10x35_1.png" });
                await AddProductAsync("Caja Ecologica  17x13x8cm", "Caja Ecologica con tapa y base  con medidas de 17x13x8 cm ", 2500M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaEcoTapaB17x13x8.png", "https://localhost:7116/images/products/CajaEcoTapaB17x13x8_1.png" });
                await AddProductAsync("Caja kraft  27x18x4cm", "Caja kraft con tapa y base  con medidas de 27x18x4 cm ", 4900M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajaTapaB27x18x4.png", "https://localhost:7116/images/products/CajaTapaB27x18x4_1.png" });
                await AddProductAsync("Bolsa Regalo 10x26x32cm", "Bolsa de regalo colores (rojo, plateado, azul, verde, dorado, fucsia, morado)  con medidas de 10x26x32 cm ", 2800M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegalo10x26x32.png",
                    "https://localhost:7116/images/products/BolsaRegalo10x26x32_1.png","https://localhost:7116/images/products/BolsaRegalo10x26x32_2.png" ,"https://localhost:7116/images/products/BolsaRegalo10x26x32_3.png" ,"https://localhost:7116/images/products/BolsaRegalo10x26x32_4.png" ,
                "https://localhost:7116/images/products/BolsaRegalo10x26x32_5.png","https://localhost:7116/images/products/BolsaRegalo10x26x32_6.png"  });
                await AddProductAsync("Bolsa Regalo  Come galletas 10x26x32cm", "Bolsa Regalo  Come galletas con medidas de 10x26x32 cm ", 4200M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegaloComeGalletas10x26x32.png" });
                await AddProductAsync("Bolsa Regalo  Tigger 10x26x32cm", "Bolsa Regalo  Tigger con medidas de 10x26x32 cm ", 4200M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegaloTigger10x26x32.png" });
                await AddProductAsync("Bolsa Regalo  Cars 10x26x32cm", "Bolsa Regalo  Cars con medidas de 10x26x32 cm ", 4200M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegaloCars10x26x32.png", "https://localhost:7116/images/products/BolsaRegaloCars10x26x32_2.png","https://localhost:7116/images/products/BolsaRegaloCars10x26x32_2.png" });
                await AddProductAsync("Bolsa Regalo  Paw  Patrol 10x26x32cm", "Bolsa Regalo  Paw  Patrol  con medidas de 10x26x32 cm ", 4200M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegaloPawPatrol10x26x32.png", "https://localhost:7116/images/products/BolsaRegaloPawPatrol10x26x32_1.png" });
                await AddProductAsync("Bolsa Regalo  Toy  Story 10x26x32cm", "Bolsa Regalo  Toy  Story   con medidas de 10x26x32 cm ", 4200M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegaloToyStory10x26x32.png", "https://localhost:7116/images/products/BolsaRegaloToyStory10x26x32_1.png", "https://localhost:7116/images/products/BolsaRegaloToyStory10x26x32_2.png" });
                await AddProductAsync("Bolsa Regalo  Hotwheels 10x26x32cm", "Bolsa Regalo  Hotwheels con medidas de 10x26x32 cm ", 4200M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegaloHotwheels10x26x32.png" });
                await AddProductAsync("Bolsa Regalo  Doctora juguetes 10x26x32cm", "Bolsa Regalo Doctora juguetes con medidas de 10x26x32 cm ", 4200M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegaloDoctoraJuguete10x26x32.png", "https://localhost:7116/images/products/BolsaRegaloDoctoraJuguete10x26x32_1.png" });
                await AddProductAsync("Bolsa Regalo  Frozen 10x26x32cm", "Bolsa Regalo Frozen  con medidas de 10x26x32 cm ", 4200M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegaloFrozen10x26x32.png", "https://localhost:7116/images/products/BolsaRegaloFrozen10x26x32_1.png", "https://localhost:7116/images/products/BolsaRegaloFrozen10x26x32_2.png" });
                await AddProductAsync("Bolsa Regalo  Marie 10x26x32cm", "Bolsa Regalo  Marie con medidas de 10x26x32 cm ", 4200M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegaloMarie10x26x32.png" });
                await AddProductAsync("Bolsa Regalo  Barbie 10x26x32cm", "Bolsa Regalo Barbie  con medidas de 10x26x32 cm ", 4200M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegaloBarbie10x26x32.png", "https://localhost:7116/images/products/BolsaRegaloBarbie10x26x32_1.png" });
                await AddProductAsync("Bolsa Regalo  Princesa Sofia 10x26x32cm", "Bolsa Regalo Princesa Sofia  con medidas de 10x26x32 cm ", 4200M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegaloPrincesaSofia10x26x32.png", "https://localhost:7116/images/products/BolsaRegaloPrincesaSofia10x26x32_1.png" });
                await AddProductAsync("Bolsa Regalo  Princesas Disney 10x26x32cm", "Bolsa Regalo Princesas Disney  con medidas de 10x26x32 cm ", 4200M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegaloPrincesasDisney10x26x32.png", "https://localhost:7116/images/products/BolsaRegaloPrincesasDisney10x26x32_1.png" });
                await AddProductAsync("Bolsa Regalo  Caballero 10x26x32cm", "Bolsa Regalo Caballero  con medidas de 10x26x32 cm ", 4200M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegaloCaballero10x26x32.png", "https://localhost:7116/images/products/BolsaRegaloCaballero10x26x32_1.png" , "https://localhost:7116/images/products/BolsaRegaloCaballero10x26x32_2.png",
                "https://localhost:7116/images/products/BolsaRegaloCaballero10x26x32_3.png","https://localhost:7116/images/products/BolsaRegaloCaballero10x26x32_4.png"});
                await AddProductAsync("Bolsa Regalo  Dama 10x26x32cm", "Bolsa Regalo Dama  con medidas de 10x26x32 cm ", 4200M, 12F, new List<string>() { "Bolsas" }, new List<string>() { "https://localhost:7116/images/products/BolsaRegaloDama10x26x32.png", "https://localhost:7116/images/products/BolsaRegaloDama10x26x32_1.png" , "https://localhost:7116/images/products/BolsaRegaloDama10x26x32_2.png",
                "https://localhost:7116/images/products/BolsaRegaloDama10x26x32_3.png", "https://localhost:7116/images/products/BolsaRegaloDama10x26x32_4.png" , "https://localhost:7116/images/products/BolsaRegaloDama10x26x32_5.png", "https://localhost:7116/images/products/BolsaRegaloDama10x26x32_6.png", "https://localhost:7116/images/products/BolsaRegaloDama10x26x32_7.png" , "https://localhost:7116/images/products/BolsaRegaloDama10x26x32_8.png",  });
                await AddProductAsync("Caja de Madera Desayuno", "Caja de Madera Grande  Desayuno Sorpresa Clásica", 9500M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajadeMaderaDesayuno.png" });
                await AddProductAsync("Caja de Madera Con Tapa", "Caja de Madera Con Tapa", 9500M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajadeMaderaConTapa.png", "https://localhost:7116/images/products/CajadeMaderaConTapa_1.png" });
                await AddProductAsync("Caja de Madera Corral", "Caja de Madera Corral", 9500M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajadeMaderaCorral.png" });
                await AddProductAsync("Caja de Madera Agarradera", "Caja de Madera Agarradera", 12500M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajadeMaderaAgarradera.png" });
                await AddProductAsync("Caja de Madera Corazón", "Caja de Madera Corazón", 10500M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajadeMaderaCorazon.png" });
                await AddProductAsync("Caja de Madera Huacal", "Caja de Madera Huacal", 9500M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajadeMaderaHuacal.png" });
                await AddProductAsync("Caja de Madera Huacal Pequeña", "Caja de Madera Huacal Pequeña", 8500M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajadeMaderaHuacalP.png" });
                await AddProductAsync("Caja de Madera Regalo", "Caja de Madera Regalo", 10500M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajadeMaderaRegalo.png" });
                await AddProductAsync("Caja de Madera Desayuno con Patas", "Caja de Madera Desayuno con Patas", 9500M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajadeMaderaDesayunoPatas.png" });
                await AddProductAsync("Caja de Madera Pequeña", "Caja de Madera Pequeña", 6000M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajadeMaderaDesayunoPequena.png" });
                await AddProductAsync("Caja de Madera Pequeña Corazón", "Caja de Madera Pequeña Corazón", 9500M, 12F, new List<string>() { "Empaques" }, new List<string>() { "https://localhost:7116/images/products/CajadeMaderaPequenaCorazon.png" });





                await AddProductAsync("Ancheta Flores y Frutas", "Ancheta en caja de madera con: Flores, 2 pines Feliz dia, Furtas, Moño", 35000M, 12F, new List<string>() { "Regalos", "Flores" }, new List<string>() { "https://localhost:7116/images/products/anchetafloresyfrutas.png" });
                await AddProductAsync("Ancheta Feliz Cumpleaños", "Ancheta en caja de madera con: Globos, Tarjeta, Botella Olpard, Pines, Dulces", 180000M, 12F, new List<string>() { "Regalos" }, new List<string>() { "https://localhost:7116/images/products/anchetafelizcumplenos.png" });
                await AddProductAsync("Ancheta Caja de madera", "Ancheta en caja de madera con: Globos, Tarjeta, Botella Ron Caldas, Cerveza, Pines, Dulces", 165000M, 12F, new List<string>() { "Regalos" }, new List<string>() { "https://localhost:7116/images/products/anchetaron.png" });
                await AddProductAsync("Ancheta Feliz Cumpleaños con Fotos", "Ancheta en caja de madera con: Globos, Tarjeta, Cervezas, Fotos, Pines, Dulces", 75000M, 12F, new List<string>() { "Regalos" }, new List<string>() { "https://localhost:7116/images/products/anchetadesyunoyfotos.png" });
                await AddProductAsync("Ancheta Empresariales", "Caja Dulcera en plastico con diversos duclces y con tarjeta", 6000M, 12F, new List<string>() { "Regalos" }, new List<string>() { "https://localhost:7116/images/products/anchetaempresarial.png" });
                await AddProductAsync("Ancheta Papá", "Ancheta en caja de madera con: globos, letrero, Vaso, Dulces, Botella Olpard", 210000M, 12F, new List<string>() { "Regalos" }, new List<string>() { "https://localhost:7116/images/products/anchetapapa.png" });
                await AddProductAsync("Ancheta Caja de Madera Papá", "Ancheta en Caja de madera con: Pines, Dulces, Globos", 35000M, 12F, new List<string>() { "Regalos" }, new List<string>() { "https://localhost:7116/images/products/anchetapaparoja.png" });
                await AddProductAsync("Ancheta Feliz día", "Ancheta en Caja de carton  con: Pines, Dulces, Globos", 32000M, 12F, new List<string>() { "Regalos" }, new List<string>() { "https://localhost:7116/images/products/anchetafelizdiaazul.png" });
                await AddProductAsync("Caja sorpresa", "Caja sorpresa con dulces, tarjetas, pines, globos", 60000M, 12F, new List<string>() { "Regalos" }, new List<string>() { "https://localhost:7116/images/products/cajasorpresa.png" });
                await AddProductAsync("Ancheta Caja Carton", "Caja regalo  con dulces, snack, dulces, Aguardiente", 65000M, 12F, new List<string>() { "Regalos" }, new List<string>() { "https://localhost:7116/images/products/cajasorpresa.png" });
                await AddProductAsync("Ancheta Caja Carton #2", "Caja regalo  con dulces marcada a tu preferencia", 30000M, 12F, new List<string>() { "Regalos" }, new List<string>() { "https://localhost:7116/images/products/anchetacajadulces.png" });
                await AddProductAsync("Desayuno Sorpresa", "Desayuno sorpresa con: bebida, fruta, sanduche, dulces, globos", 70000M, 12F, new List<string>() { "Regalos" }, new List<string>() { "https://localhost:7116/images/products/desayunosorpresa.png" });

                await AddProductAsync("Ramo de girasoles", "Ramo de girasoles y rosas naranjadas", 35000M, 12F, new List<string>() { "Regalos", "Flores" }, new List<string>() { "https://localhost:7116/images/products/ramogirasol.png" });
                await AddProductAsync("Ancheta de girasoles", "Ancheta de Girasoles, Frutas y Vino", 65000M, 12F, new List<string>() { "Regalos", "Flores" }, new List<string>() { "https://localhost:7116/images/products/anchetagirasolesyfrutas.png" });
                await AddProductAsync("Ancheta de Rosas", "Ancheta de Rosas y Vino", 65000M, 12F, new List<string>() { "Regalos", "Flores" }, new List<string>() { "https://localhost:7116/images/products/anchetafloresyvino.png" });
                await AddProductAsync("Desayuno Sorpresa con Rosas", "Desayuno Sorpres con: globo, tarjeta, bebidas, cereal, fruta, sanduche y arreglo de rosas", 150000M, 12F, new List<string>() { "Regalos", "Flores" }, new List<string>() { "https://localhost:7116/images/products/desayunosorpresarosas.png" });
                await AddProductAsync("Arreglo Floral con Chocolates", "Arreglo de rosas y girasoles con chocolates y tarjeta", 150000M, 12F, new List<string>() { "Regalos", "Flores" }, new List<string>() { "https://localhost:7116/images/products/arreglofloresycgocolates.png" });
                await AddProductAsync("Arreglo Floral 15 años ", "Arreglo de rosas  para 15 años", 150000M, 12F, new List<string>() { "Regalos", "Flores" }, new List<string>() { "https://localhost:7116/images/products/arregloflores15.png" });
                await AddProductAsync("Arreglo de Rosas y Chocolates ", "Arreglo de rosas  con chocolates", 150000M, 12F, new List<string>() { "Regalos", "Flores" }, new List<string>() { "https://localhost:7116/images/products/rosasconcholate.png" });
                await AddProductAsync("Arreglo de Rosas blancas ", "Arreglo de rosas  blancas", 150000M, 12F, new List<string>() { "Regalos", "Flores" }, new List<string>() { "https://localhost:7116/images/products/rosasconcholate.png" });
                await AddProductAsync("Arreglo de Rosas azules ", "Arreglo de rosas  azules", 34000M, 12F, new List<string>() { "Regalos", "Flores" }, new List<string>() { "https://localhost:7116/images/products/rosasazules.png" });
                await AddProductAsync("Desayuno Sorpreza Caja", "Desayuno sorpresa con bebida, sanduche, fruta, cereal, Panqueque, globos y rosas", 150000M, 12F, new List<string>() { "Regalos", "Flores" }, new List<string>() { "https://localhost:7116/images/products/desayunocaja.png" });

                await AddProductAsync("Peluche Vaca", "Peluche Vaca", 750000M, 12F, new List<string>() {  "Peluches" }, new List<string>() { "https://localhost:7116/images/products/peluchevaca.png" });
                await AddProductAsync("Peluche Elefante Pequeño", "Peluche Elefante Pequeño", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/pelucheelefantepqeue.png" });
                await AddProductAsync("Peluche Unicornio", "Peluche Unicornio", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/pelucheunicornio.png" });
                await AddProductAsync("Peluche Oso Peresozo", "Peluche UnicoOso Peresozo", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/pelucheosopersozo.png" });
                await AddProductAsync("Peluche Oso Panda", "Peluche UnicoOso Panda", 160000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/pelucheosopanda.png" });
                await AddProductAsync("Peluche Mono", "Peluche Mono", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/peluchemono.png" });
                await AddProductAsync("Peluche Elefante Corona", "Peluche Elefante con corona", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/pelucheelefantecorona.png" });
                await AddProductAsync("Peluche Elefante Gris", "Peluche Elefante gris", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/pelucheelefantegrisoscuro.png" });
                await AddProductAsync("Peluche Marrano", "Peluche Marrano ", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/peluchemarrano.png" });
                await AddProductAsync("Peluche Conejo", "Peluche Conejo ", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/pelucheconejorosa.png" });
                await AddProductAsync("Peluche Canguro", "Peluche Canguro ", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/peluchecanguro.png" });
                await AddProductAsync("Peluche Caballo", "Peluche Caballo ", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/peluchecaballo.png" });
                await AddProductAsync("Peluche Pinguino con gorra", "Peluche Pinguino con gorra ", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/peluchepinguinogorra.png" });
                await AddProductAsync("Peluche Aguacate", "Peluche Aguacate ", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/pelucheaguacate.png" });
                await AddProductAsync("Peluche Elefante rosa", "Peluche Elefante rosa ", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/pelucheelefanterosa.png" });
                await AddProductAsync("Peluche Elefante con camisa", "Peluche Elefante con camisa ", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/pelucheelefante.png" });
                await AddProductAsync("Peluche Pinguino", "Peluche Pinguino ", 35000M, 12F, new List<string>() { "Peluches" }, new List<string>() { "https://localhost:7116/images/products/peluchepinguino.png" });

                await AddProductAsync("Serum Liftactiv B3 l", "Serum Liftactiv B3 Specialist Vichy Anti-manchas x30 ml ", 210000M, 12F, new List<string>() { "Belleza" }, new List<string>() { "https://localhost:7116/images/products/SerumLiftactiv.png" });
                await AddProductAsync("Circadien Serum Reparador ", "Circadien Serum Reparador Emulsion Hidrisage Frasco X 40Ml ", 210000M, 12F, new List<string>() { "Belleza" }, new List<string>() { "https://localhost:7116/images/products/CircadienSerum.png" });
                await AddProductAsync("Crema Avene Dermabsolu ", "Crema Avene Dermabsolu Balsamo De Noche 40Ml ", 220000M, 12F, new List<string>() { "Belleza" }, new List<string>() { "https://localhost:7116/images/products/CremaAvene.png" });
                await AddProductAsync("Aceite Corporal ", "Aceite Corporal Hidratante Cool Nature x250 ml", 48000M, 12F, new List<string>() { "Belleza" }, new List<string>() { "https://localhost:7116/images/products/CoolNature.png" });
                await AddProductAsync("Aceite BIO OIL ", "Aceite BIO OIL Corporal Cuidado de Piel PurCellin x125Ml ", 55000M, 12F, new List<string>() { "Belleza" }, new List<string>() { "https://localhost:7116/images/products/AceiteBIOOIL.png" });
                await AddProductAsync("Crema Aderma ", "Crema Aderma Emoliente Exomega Control X200ml ", 100000M, 12F, new List<string>() { "Belleza" }, new List<string>() { "https://localhost:7116/images/products/CremaAderma.png" });
                await AddProductAsync("Cera Para Peinar Squash ", "Cera Para Peinar Squash For Men Efecto Mate x 85Gr", 22000M, 12F, new List<string>() { "Belleza" }, new List<string>() { "https://localhost:7116/images/products/Squash.png" });
                await AddProductAsync("Tratamiento Herbal ", "Tratamiento Herbal Essences Mascarilla Reconstructiva Aceite de Argan x 300Ml", 30000M, 12F, new List<string>() { "Belleza" }, new List<string>() { "https://localhost:7116/images/products/TratamientoHerbal.png" });
                await AddProductAsync("Cera Moldeadora", "Cera Moldeadora Got2B Phenomenal En Pasta Nivel De Fijacion 4 100ml", 33000M, 12F, new List<string>() { "Belleza" }, new List<string>() { "https://localhost:7116/images/products/CeraMoldeadora.png" });
                await AddProductAsync("Shampoo Organix", "Shampoo Organix Charcoal Detox x385ml", 60000M, 12F, new List<string>() { "Belleza" }, new List<string>() { "https://localhost:7116/images/products/ShampooOrganix.png" });
                await AddProductAsync("Shampoo OGX", "SHAMPOO OGX NOURISHING COCONUT MILK X385ML", 60000M, 12F, new List<string>() { "Belleza" }, new List<string>() { "https://localhost:7116/images/products/ShampooOGX.png" });
                await AddProductAsync("Shampoo Renewing", "Shampoo OGX Renewing arcan oil of morocco X385ML", 60000M, 12F, new List<string>() { "Belleza" }, new List<string>() { "https://localhost:7116/images/products/ShampooRenewing.png" });

                await AddProductAsync("Hebilla para el Cabello", "Hebilla para el Cabello Surtida Scunci Diseño de Moño x6 Und", 40000M, 12F, new List<string>() { "Belleza", "Accesorios" }, new List<string>() { "https://localhost:7116/images/products/HebillaparaelCabello.png" });
                await AddProductAsync("Balaca Unicornio", "Balaca Unicornio x 1und", 9000M, 12F, new List<string>() { "Belleza", "Accesorios" }, new List<string>() { "https://localhost:7116/images/products/BalacaUnicornio.png" });
                await AddProductAsync("Cepillo Salon Expert", "Cepillo Salon Expert Wave Vent Profesional x1 und", 9000M, 12F, new List<string>() { "Belleza", "Accesorios" }, new List<string>() { "https://localhost:7116/images/products/CepilloSalonExpert.png" });
                await AddProductAsync("Pack Cepillo ", "Pack Cepillo Conair Ovalado Mediano + Cepillo Conair Ovalado Pequeño x2und", 29000M, 12F, new List<string>() { "Belleza", "Accesorios" }, new List<string>() { "https://localhost:7116/images/products/PackCepillo.png" });
                await AddProductAsync("Cortadora de Cabello", "Cortadora de Cabello Braun 6 en 1 Recargable x1 und", 140000M, 12F, new List<string>() { "Belleza", "Accesorios" }, new List<string>() { "https://localhost:7116/images/products/CortadoradeCabello.png" });
                await AddProductAsync("Rizador de Cabello", "Rizador de Cabello Conair Triple Cilindro Doble Cerámica x 1 Unidad", 140000M, 12F, new List<string>() { "Belleza", "Accesorios" }, new List<string>() { "https://localhost:7116/images/products/RizadordeCabello.png" });
                await AddProductAsync("Secador Para Cabello", "Secador Para Cabello Profesional Gama Mistral Titanium", 140000M, 12F, new List<string>() { "Belleza", "Accesorios" }, new List<string>() { "https://localhost:7116/images/products/SecadorParaCabello.png" });
                await AddProductAsync("Paletas rostro", "Paletas rostro Ushas", 35000, 12F, new List<string>() { "Belleza", }, new List<string>() { "https://localhost:7116/images/products/Paletasrostro.png" });
                await AddProductAsync("Rubor corazon", "Rubor corazon karite", 8000M, 12F, new List<string>() { "Belleza"}, new List<string>() { "https://localhost:7116/images/products/Ruborcorazon.png" });
                await AddProductAsync("Rubor 3 En 1", "Rubor 3 En 1 Febble", 17000M, 12F, new List<string>() { "Belleza" }, new List<string>() { "https://localhost:7116/images/products/Rubor3En1.png" });
                await AddProductAsync("Iluminador X3", "Iluminador X3 ping up", 30000M, 12F, new List<string>() { "Belleza" }, new List<string>() { "https://localhost:7116/images/products/IluminadorX3.png" });
                await AddProductAsync("Kit brochas ", "Kit brochas ruby face neon", 30000M, 12F, new List<string>() { "Belleza", "Accesorios" }, new List<string>() { "https://localhost:7116/images/products/Kitbrochas.png" });
                await AddProductAsync("Kit brochas ojos pro negro", "Kit brochas ojos pro negro", 50000M, 12F, new List<string>() { "Belleza", "Accesorios" }, new List<string>() { "https://localhost:7116/images/products/Kitbrochasojos.png" });
                await AddProductAsync("Blender Huevo", "Blender Huevo Ushas", 5000M, 12F, new List<string>() { "Belleza", "Accesorios" }, new List<string>() { "https://localhost:7116/images/products/BlenderHuevo.png" });
                await AddProductAsync("Brocha Ind Difuminar", "Brocha Ind Difuminar", 15000M, 12F, new List<string>() { "Belleza", "Accesorios" }, new List<string>() { "https://localhost:7116/images/products/BrochaIndDifuminar.png" });
                await AddProductAsync("Aplicador pestañas", "Aplicador pestañas X24", 10000M, 12F, new List<string>() { "Belleza", "Accesorios" }, new List<string>() { "https://localhost:7116/images/products/Aplicadorpestanas.png" });
                await AddProductAsync("Tarro Blender", "Tarro Blender X6 ", 15000M, 12F, new List<string>() { "Belleza", "Accesorios" }, new List<string>() { "https://localhost:7116/images/products/TarroBlender.png" });

                await AddProductAsync("Papel Globo X6", "Papel Globo X6 Colores surtidos ", 2000M, 12F, new List<string>() { "Papeles"}, new List<string>() { "https://localhost:7116/images/products/PapelGlobo.png" });
                await AddProductAsync("Papel Crepe X6", "Papel Crepe X6 Colores surtidos ", 2500M, 12F, new List<string>() { "Papeles" }, new List<string>() { "https://localhost:7116/images/products/PapelCrepe.png" });

                await AddProductAsync("Globo Latex R-5 X20", "Globo Latex R-5 X20 disponible en todos los cores y colores surtidos", 4000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor5x20.png" });
                await AddProductAsync("Globo Latex R-5 X50", "Globo Latex R-5 X50 disponible en todos los cores y colores surtidos", 4000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor5x50.png" });
                await AddProductAsync("Globo Latex R-9 X50", "Globo Latex R-9 X50 disponible en todos los cores y colores surtidos", 12000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor9x50.png" });
                await AddProductAsync("Globo Latex R-12 X12", "Globo Latex R-9 X12 disponible en todos los cores y colores surtidos", 6000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor12x12.png" });
                await AddProductAsync("Globo Latex R-12 X20", "Globo Latex R-9 X20 disponible en todos los cores y colores surtidos", 8000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor12x20.png" });
                await AddProductAsync("Globo Latex R-12 X50", "Globo Latex R-9 X50 disponible en todos los cores y colores surtidos", 20000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor12x50.png" });
                await AddProductAsync("Globo Latex R-18 X6", "Globo Latex R-18 X6 disponible en todos los cores y colores surtidos", 10000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor18x6.png" });
                await AddProductAsync("Globo Latex Satin y Metal R-12 X12", "Globo Latex Satin y Metal R-12 X12 en color negro, dorado, plateado", 6000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor12x12satin.png" });
                await AddProductAsync("Globo Latex Metalizado R-5 X20", "Globo Latex Metalizado R-5 X20 en todos los colores y surtido", 9000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor5x20metalizado.png" });
                await AddProductAsync("Globo Latex Metalizado R-5 X50", "Globo Latex Metalizado R-5 X50 en todos los colores y surtido", 22000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor5x50metalizado.png" });
                await AddProductAsync("Globo Latex Metalizado R-12 X50", "Globo Latex Metalizado R-12 X50 en todos los colores y surtido", 50000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor12x50metalizado.png" });
                await AddProductAsync("Globo Latex Metalizado R-12 X12", "Globo Latex Metalizado R-12 X12 en todos los colores y surtido", 15000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor12x12metalizado.png" });
                await AddProductAsync("Globo Latex  R-5  Unidad", "Globo Latex  R-5  Unidad en todos los colores", 300M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor5unidad.png" });
                await AddProductAsync("Globo Latex  R-12  Unidad", "Globo Latex  R-12  Unidad en todos los colores", 500M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor12unidad.png" });
                await AddProductAsync("Globo Latex  R-18  Unidad", "Globo Latex  R-18  Unidad en todos los colores", 1900M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor18unidad.png" });
                await AddProductAsync("Globo Latex Transparente  R-5  X50", "Globo Latex  R-5  X50", 1000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor5x50transparente.png" });
                await AddProductAsync("Globo Latex Transparente  R-12  X50", "Globo Latex  R-12  X50", 2000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor12x50transparente.png" });
                await AddProductAsync("Globo Latex Transparente  R-12  X12", "Globo Latex  R-12  X12", 7000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor12x12transparente.png" });
                await AddProductAsync("Globo Latex Transparente  R-18  X6", "Globo Latex  R-18  X6", 10000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor18x6transparente.png" });
                await AddProductAsync("Globo Latex Transparente  R-5  Unidad", "Globo Latex  R-5  Unidad", 300M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor5Unitransparente.png" });
                await AddProductAsync("Globo Latex Transparente  R-12  Unidad", "Globo Latex  R-12  Unidad", 500M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor12Unitransparente.png" });
                await AddProductAsync("Globo Latex Transparente  R-18  Unidad", "Globo Latex  R-18  Unidad", 2000M, 12F, new List<string>() { "Globos" }, new List<string>() { "https://localhost:7116/images/products/Globor18Unitransparente.png" });

                await AddProductAsync("Letrero feliz cumpleaños", "Letrero feliz cumpleaños", 4000M, 12F, new List<string>() { "Decoración" }, new List<string>() { "https://localhost:7116/images/products/letrerofelizcumpleanos.png" });
                await AddProductAsync("Letrero feliz cumpleaños rosado", "Letrero feliz cumpleaños rosado", 4000M, 12F, new List<string>() { "Decoración" }, new List<string>() { "https://localhost:7116/images/products/letrerofelizcumpleanosrosado.png" });
                await AddProductAsync("Globlo Letrero Feliz Cumpleaños", "Globlo Letrero Feliz Cumpleaños", 10000M, 12F, new List<string>() { "Decoración", "Globos" }, new List<string>() { "https://localhost:7116/images/products/globlolfelizcumpleanosrosado.png" });
                await AddProductAsync("Globlo Letrero Happy Birthday", "Globlo Letrero Happy Birthday", 15000M, 12F, new List<string>() { "Decoración", "Globos" }, new List<string>() { "https://localhost:7116/images/products/globlohappybirthday.png" });
                await AddProductAsync("Globo Letra 40CM", "Globo Letra 40CM disponible todas las letras en color dorado, palo de rosa, plateado, negro", 4000M, 12F, new List<string>() { "Decoración" }, new List<string>() { "https://localhost:7116/images/products/globoletras40.png" });
                await AddProductAsync("Globo Letra 80CM", "Globo Letra  80CMdisponible todas las letras en color dorado, palo de rosa, plateado, negro", 9000M, 12F, new List<string>() { "Decoración" }, new List<string>() { "https://localhost:7116/images/products/globoletras80.png" });
                await AddProductAsync("Globo Número 40CM", "Globo Número 40CM disponible todas las números en color dorado, palo de rosa, plateado, negro", 4000M, 12F, new List<string>() { "Decoración" }, new List<string>() { "https://localhost:7116/images/products/globonumeros40.png" });
                await AddProductAsync("Globo Número 80CM", "Globo Número 80CM disponible todas las múmeros en color dorado, palo de rosa, plateado, negro", 4000M, 12F, new List<string>() { "Decoración" }, new List<string>() { "https://localhost:7116/images/products/globonumeros80.png" });

                await AddProductAsync("Nutella Tarro 140Gr", "Nutella Tarro 140Gr", 10000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/NutellaTarro.png" });
                await AddProductAsync("Montblanc unidad 60Gr", "Montblanc unidad 60Gr", 5000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/Montblanc.png" });
                await AddProductAsync("Jet Cookies and Cream 50Gr", "Jet Cookies and Cream 50Gr", 3000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/JetCookiesandCream.png" });
                await AddProductAsync("Golochips", "Golochips", 1200M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/Golochips.png" });
                await AddProductAsync("Choco Line", "Choco Line", 1700M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/ChocoLine.png" });
                await AddProductAsync("BurbuJet Cookies And Cream", "BurbuJet Cookies And Cream", 2800M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/BurbuJetCookiesAndCream.png" });
                await AddProductAsync("Bianchi Barra", "Bianchi Barra", 800M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/BianchiBarra.png" });
                await AddProductAsync("Chocolate Gol", "Chocolate Gol", 1100M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/ChocolateGol.png" });
                await AddProductAsync("Caja x4 Unidades FerreroRocher", "Caja x4 Unidades FerreroRocher", 10000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/Cajax4UnidadesFerreroRocher.png" });
                await AddProductAsync("Chocolates adro x3 Unidades", "Chocolates adro x3 Unidades", 6000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/Chocolatesadrox3Unidades.png" });
                await AddProductAsync("Jumbo pistacho", "Jumbo pistacho", 9000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/Jumbopistacho.png" });
                await AddProductAsync("Jumbo Browniew", "Jumbo Brownie", 9000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/JumboBrowniew.png" });
                await AddProductAsync("Jumbo Bites", "Jumbo Bites", 2500M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/JumboBites.png" });
                await AddProductAsync("Jet Crema", "Jet Crema", 6000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/JetCrema.png" });
                await AddProductAsync("Trululu Sabore Unidad", "Trululu Sabore Unidad", 1000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/TrululuSaboreUnidad.png" });
                await AddProductAsync("Trululu Fresitas Unidad", "Trululu Fresitas Unidad", 2000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/TrululuFresitasUnidad.png" });
                await AddProductAsync("Gomas Colageno Colombina", "Gomas Colageno Colombina", 3000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/GomasColagenoColombina.png" });
                await AddProductAsync("Gomas Trolli Banana", "Gomas Trolli Banana", 2000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/GomasTrolliBanana.png" });
                await AddProductAsync("Gomas Corazon Vidal", "Gomas Corazon Vidal", 2800M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/GomasCorazonVidal.png" });
                await AddProductAsync("Trululu Chocolores", "Trululu Chocolores", 1100M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/TrululuChocolores.png" });
                await AddProductAsync("Gomas Grissly Tiburon", "Gomas Grissly Tiburon", 2000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/GomasGrisslyTiburon.png" });
                await AddProductAsync("Gomas Pikis", "Gomas Pikis ", 2000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/GomasPikis.png" });
                await AddProductAsync("ChocMelos Bessos", "ChocMelos Bessos ", 1200M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/ChocMelosBessos.png" });
                await AddProductAsync("Millows Metro", "Millows Metro ", 1200M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/MillowsMetro.png" });
                await AddProductAsync("Millows Surtido", "Millows Surtido ", 3000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/MillowsSurtido.png" });
                await AddProductAsync("M&M Cafe", "M&M Cafe", 4000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/M&MCafe.png" });
                await AddProductAsync("M&M Amarillo", "M&M Amarillo", 4000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/M&MAmarillo.png" });
                await AddProductAsync("Chicle Orbit Tarro", "Chicle Orbit Tarro", 9000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/ChicleOrbitTarro.png" });
                await AddProductAsync("Hershey Cookies Gigante", "Hershey Cookies Gigante", 13000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/HersheyCookiesGigante.png" });
                await AddProductAsync("Snickers", "Snickers", 4000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/Snickers.png" });
                await AddProductAsync("M&M Tarro", "M&M Tarro", 5000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/M&MTarro.png" });
                await AddProductAsync("MilkyWay", "MilkyWay", 4000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/MilkyWay.png" });
                await AddProductAsync("M&M CholateBar", "M&M CholateBar", 12000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/M&MCholateBar.png" });
                await AddProductAsync("Hershey Cookies ", "Hershey Cookies ", 4000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/HersheyCookies.png" });
                await AddProductAsync("Almendra Francesa ", "Almendra Francesa ", 5000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/AlmendraFrancesa.png" });
                await AddProductAsync("Trident Total ", "Trident Total ", 3000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/TridentTotal.png" });
                await AddProductAsync("Trident Freshmint ", "Trident Freshmint ", 4000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/TridentFreshmint.png" });
                await AddProductAsync("DoubleMint ", "DoubleMint", 2000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/DoubleMint.png" });
                await AddProductAsync("Chicle Extra ", "Chicle Extra", 3000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/ChicleExtra.png" });
                await AddProductAsync("Sparkies Sobre", "Sparkies Sobre", 1200M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/SparkiesSobre.png" });
                await AddProductAsync("Quipitos ", "Quipitos ", 1000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/Quipitos.png" });
                await AddProductAsync("Okaloka Ovnis ", "Okaloka Ovnis ", 1200M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/OkalokaOvnis.png" });
                await AddProductAsync("Okaloka Fucsion ", "Okaloka Fucsion ", 1200M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/OkalokaFucsion.png" });
                await AddProductAsync("Barra Chao cereza", "Barra Chao cereza ", 1200M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/BarraChaocereza.png" });
                await AddProductAsync("Menta Sin Azucar", "Menta Sin Azucar ", 8000M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/MentaSinAzucar.png" });
                await AddProductAsync("Max Bara", "Max Bara ", 1200M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/MaxBara.png" });
                await AddProductAsync("SuperCoco Bara", "SuperCoco Bara ", 1500M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/SuperCocoBara.png" });
                await AddProductAsync("Okaloka Chicle ", "Okaloka Chicle ", 1200M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/OkalokaChicle.png" });
                await AddProductAsync("CandyRanch ", "CandyRanch  ", 900M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/CandyRanch.png" });
                await AddProductAsync("Frunas Frugosas ", "Frunas Frugosas", 2900M, 12F, new List<string>() { "Dulces" }, new List<string>() { "https://localhost:7116/images/products/FrunasFrugosas.png" });




            }
        }

        private async Task AddProductAsync(string name,  string description, decimal price, float stock, List<string> categories, List<string> images)
        {
            Products prodcut = new()
            {
                
                Name = name,
                Price = price,
                Stock = stock,
                Description=description,
                ProductCategories = new List<ProductCategory>(),
                ProductImages = new List<ProductImage>()
            };

            foreach (string? category in categories)
            {
                prodcut.ProductCategories.Add(new ProductCategory { Category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category) });
            }


            foreach (string? image in images)
            {
                //blob
                //Guid imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\products\\{image}", "products");
                prodcut.ProductImages.Add(new ProductImage { ImageFullPath = image, Name=name+prodcut.ImagesNumber.ToString() });
            }

            _context.Products.Add(prodcut);
        }


    }
}
