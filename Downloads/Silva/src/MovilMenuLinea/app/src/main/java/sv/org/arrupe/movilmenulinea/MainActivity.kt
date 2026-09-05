package sv.org.arrupe.retrofitapi

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.viewModels
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.LazyRow
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Search
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.runtime.saveable.rememberSaveable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import coil.compose.rememberAsyncImagePainter
import sv.org.arrupe.retrofitapi.model.Platillo
import sv.org.arrupe.retrofitapi.viewmodel.PlatilloViewModel
import java.util.Locale

private val RosaPrincipal = Color(0xFFE00045)
private val VerdeLima = Color(0xFFC8E608)
private val Crema = Color(0xFFFFEDBA)

private val RosaOscuro = Color(0xFFB50038)
private val TextoOscuro = Color(0xFF242424)
private val TextoSecundario = Color(0xFF686868)
private val FondoClaro = Color(0xFFFFFBF1)


class MainActivity : ComponentActivity() {

    private val viewModel: PlatilloViewModel by viewModels()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContent {

            val colorScheme = lightColorScheme(
                primary = RosaPrincipal,
                onPrimary = Color.White,

                secondary = VerdeLima,
                onSecondary = TextoOscuro,

                tertiary = Crema,
                onTertiary = TextoOscuro,

                background = FondoClaro,
                onBackground = TextoOscuro,

                surface = Color.White,
                onSurface = TextoOscuro,

                surfaceVariant = Crema,
                onSurfaceVariant = TextoSecundario,

                error = Color(0xFFB3261E)
            )

            MaterialTheme(
                colorScheme = colorScheme
            ) {

                val navController = rememberNavController()

                val platillosList by viewModel.platillosList.collectAsState()
                val errorMessage by viewModel.errorMessage.collectAsState()

                NavHost(
                    navController = navController,
                    startDestination = "lista_platillos"
                ) {
                    composable("lista_platillos") {

                        PlatilloListScreen(
                            platillosList = platillosList,
                            errorMessage = errorMessage,

                            onRetry = {
                                viewModel.fetchPlatillos()
                            },

                            onPlatilloClick = { platilloId ->

                                navController.navigate(
                                    "detalle_platillo/$platilloId"
                                )
                            }
                        )
                    }
                    composable(
                        route = "detalle_platillo/{platilloId}",
                        arguments = listOf(
                            navArgument("platilloId") {
                                type = NavType.IntType
                            }
                        )
                    ) { backStackEntry ->

                        val platilloId =
                            backStackEntry.arguments?.getInt("platilloId") ?: -1

                        val platilloSeleccionado =
                            platillosList.find {
                                it.idPlatillo == platilloId
                            }

                        if (platilloSeleccionado != null) {

                            PlatilloDetailScreen(
                                platillo = platilloSeleccionado,

                                onBackClick = {
                                    navController.popBackStack()
                                }
                            )
                        }
                    }
                }
            }
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun PlatilloListScreen(
    platillosList: List<Platillo>,
    errorMessage: String?,
    onRetry: () -> Unit,
    onPlatilloClick: (Int) -> Unit
) {

    var searchQuery by rememberSaveable {
        mutableStateOf("")
    }

    var selectedCategory by rememberSaveable {
        mutableStateOf("Todas")
    }

    val categorias = remember(platillosList) {

        listOf("Todas") +
                platillosList
                    .mapNotNull { it.categoriaNombre }
                    .filter { it.isNotBlank() }
                    .distinct()
                    .sorted()
    }

    val platillosFiltrados = remember(
        platillosList,
        searchQuery,
        selectedCategory
    ) {

        platillosList.filter { platillo ->

            val coincideNombre =
                searchQuery.isBlank() ||
                        platillo.nombre
                            ?.contains(
                                searchQuery,
                                ignoreCase = true
                            ) == true

            val coincideCategoria =
                selectedCategory == "Todas" ||
                        platillo.categoriaNombre
                            ?.equals(
                                selectedCategory,
                                ignoreCase = true
                            ) == true

            coincideNombre && coincideCategoria
        }
    }


    Scaffold(

        containerColor = FondoClaro,

        topBar = {

            TopAppBar(

                title = {

                    Column {

                        Text(
                            text = "MENÚ",
                            fontWeight = FontWeight.ExtraBold,
                            fontSize = 25.sp,
                            color = RosaPrincipal
                        )

                        Text(
                            text = "Descubre nuestros platillos",
                            fontSize = 12.sp,
                            color = TextoSecundario
                        )
                    }
                },

                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = FondoClaro
                )
            )
        }

    ) { innerPadding ->

        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(innerPadding)
        ) {

            OutlinedTextField(

                value = searchQuery,

                onValueChange = {
                    searchQuery = it
                },

                modifier = Modifier
                    .fillMaxWidth()
                    .padding(
                        start = 16.dp,
                        end = 16.dp,
                        top = 8.dp,
                        bottom = 12.dp
                    ),

                placeholder = {
                    Text("Buscar un platillo...")
                },

                leadingIcon = {
                    Icon(
                        imageVector = Icons.Default.Search,
                        contentDescription = "Buscar"
                    )
                },

                singleLine = true,

                shape = RoundedCornerShape(18.dp),

                colors = OutlinedTextFieldDefaults.colors(
                    focusedBorderColor = RosaPrincipal,
                    unfocusedBorderColor = Color.LightGray,
                    focusedContainerColor = Color.White,
                    unfocusedContainerColor = Color.White
                )
            )

            Text(
                text = "Categorías",
                modifier = Modifier.padding(
                    horizontal = 16.dp
                ),
                fontSize = 15.sp,
                fontWeight = FontWeight.Bold
            )

            Spacer(
                modifier = Modifier.height(8.dp)
            )


            LazyRow(
                contentPadding = PaddingValues(
                    horizontal = 16.dp
                ),
                horizontalArrangement = Arrangement.spacedBy(8.dp)
            ) {

                items(categorias) { categoria ->

                    FilterChip(

                        selected = selectedCategory == categoria,

                        onClick = {
                            selectedCategory = categoria
                        },

                        label = {
                            Text(
                                text = categoria,
                                fontWeight = FontWeight.SemiBold
                            )
                        },

                        colors = FilterChipDefaults.filterChipColors(

                            selectedContainerColor =
                                RosaPrincipal,

                            selectedLabelColor =
                                Color.White,

                            containerColor =
                                Color.White
                        )
                    )
                }
            }


            Spacer(
                modifier = Modifier.height(14.dp)
            )

            if (
                errorMessage != null &&
                platillosList.isEmpty()
            ) {

                Box(
                    modifier = Modifier.fillMaxSize(),
                    contentAlignment = Alignment.Center
                ) {

                    Column(
                        horizontalAlignment =
                            Alignment.CenterHorizontally
                    ) {

                        Text(
                            text = errorMessage,
                            color = MaterialTheme.colorScheme.error
                        )

                        Spacer(
                            modifier = Modifier.height(16.dp)
                        )

                        Button(
                            onClick = onRetry,
                            colors = ButtonDefaults.buttonColors(
                                containerColor = RosaPrincipal
                            )
                        ) {
                            Text("Reintentar")
                        }
                    }
                }

            } else if (platillosList.isEmpty()) {

                Box(
                    modifier = Modifier.fillMaxSize(),
                    contentAlignment = Alignment.Center
                ) {

                    CircularProgressIndicator(
                        color = RosaPrincipal
                    )
                }

            } else if (platillosFiltrados.isEmpty()) {

                Box(
                    modifier = Modifier
                        .fillMaxSize()
                        .padding(32.dp),
                    contentAlignment = Alignment.Center
                ) {

                    Column(
                        horizontalAlignment =
                            Alignment.CenterHorizontally
                    ) {

                        Text(
                            text = "🍽️",
                            fontSize = 50.sp
                        )

                        Spacer(
                            modifier = Modifier.height(12.dp)
                        )

                        Text(
                            text = "No encontramos platillos",
                            fontWeight = FontWeight.Bold,
                            fontSize = 18.sp
                        )

                        Text(
                            text = "Prueba con otro nombre o categoría.",
                            color = TextoSecundario
                        )
                    }
                }

            } else {

                Text(
                    text = "${platillosFiltrados.size} platillos",
                    modifier = Modifier.padding(
                        horizontal = 16.dp
                    ),
                    color = TextoSecundario,
                    fontSize = 13.sp
                )

                Spacer(
                    modifier = Modifier.height(8.dp)
                )


                LazyColumn(

                    modifier = Modifier.fillMaxSize(),

                    contentPadding = PaddingValues(
                        start = 16.dp,
                        end = 16.dp,
                        bottom = 24.dp
                    ),

                    verticalArrangement =
                        Arrangement.spacedBy(14.dp)

                ) {

                    items(
                        items = platillosFiltrados,
                        key = {
                            it.idPlatillo
                        }
                    ) { platillo ->

                        PlatilloCard(
                            platillo = platillo,

                            onClick = {
                                onPlatilloClick(
                                    platillo.idPlatillo
                                )
                            }
                        )
                    }
                }
            }
        }
    }
}

@Composable
fun PlatilloCard(
    platillo: Platillo,
    onClick: () -> Unit
) {

    Card(

        modifier = Modifier
            .fillMaxWidth()
            .clickable {
                onClick()
            },

        shape = RoundedCornerShape(22.dp),

        colors = CardDefaults.cardColors(
            containerColor = Color.White
        ),

        elevation = CardDefaults.cardElevation(
            defaultElevation = 3.dp
        )
    ) {

        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(10.dp),

            verticalAlignment =
                Alignment.CenterVertically
        ) {

            Image(

                painter = rememberAsyncImagePainter(
                    model = platillo.imagenUrl
                        ?: "https://via.placeholder.com/300"
                ),

                contentDescription =
                    platillo.nombre ?: "Platillo",

                modifier = Modifier
                    .size(105.dp)
                    .clip(
                        RoundedCornerShape(18.dp)
                    ),

                contentScale = ContentScale.Crop
            )


            Spacer(
                modifier = Modifier.width(14.dp)
            )

            Column(
                modifier = Modifier.weight(1f)
            ) {

                platillo.categoriaNombre?.let {

                    Text(
                        text = it.uppercase(),
                        fontSize = 10.sp,
                        fontWeight = FontWeight.Bold,
                        color = RosaPrincipal
                    )
                }


                Spacer(
                    modifier = Modifier.height(3.dp)
                )


                Text(
                    text = platillo.nombre
                        ?: "Sin nombre",

                    fontSize = 19.sp,

                    fontWeight =
                        FontWeight.ExtraBold,

                    color = TextoOscuro
                )


                Spacer(
                    modifier = Modifier.height(5.dp)
                )


                platillo.descripcion?.let {

                    Text(
                        text = it,

                        fontSize = 12.sp,

                        color = TextoSecundario,

                        maxLines = 2
                    )
                }


                Spacer(
                    modifier = Modifier.height(8.dp)
                )


                Row(
                    verticalAlignment =
                        Alignment.CenterVertically
                ) {

                    Text(
                        text = "$${
                            String.format(
                                Locale.US,
                                "%.2f",
                                platillo.precio
                            )
                        }",

                        fontSize = 18.sp,

                        fontWeight =
                            FontWeight.ExtraBold,

                        color = RosaPrincipal
                    )


                    Spacer(
                        modifier = Modifier.width(10.dp)
                    )

                    platillo.tiempoPreparacion?.let {

                        Text(
                            text = "⏱ ${it} min",
                            fontSize = 11.sp,
                            color = TextoSecundario
                        )
                    }
                }


                Spacer(
                    modifier = Modifier.height(5.dp)
                )

                platillo.estado?.let { estado ->

                    val disponible =
                        estado.equals(
                            "Disponible",
                            ignoreCase = true
                        )

                    Surface(

                        shape = RoundedCornerShape(
                            50.dp
                        ),

                        color =
                            if (disponible)
                                VerdeLima.copy(alpha = 0.35f)
                            else
                                Color(0xFFFFDAD6)
                    ) {

                        Text(

                            text = estado,

                            modifier = Modifier.padding(
                                horizontal = 9.dp,
                                vertical = 4.dp
                            ),

                            fontSize = 10.sp,

                            fontWeight =
                                FontWeight.Bold,

                            color =
                                if (disponible)
                                    Color(0xFF426000)
                                else
                                    Color(0xFF8C1D18)
                        )
                    }
                }
            }
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun PlatilloDetailScreen(
    platillo: Platillo,
    onBackClick: () -> Unit
) {

    Scaffold(
        containerColor = FondoClaro,

        topBar = {
            TopAppBar(
                title = {
                    Column {
                        Text(
                            text = "MENÚ",
                            fontWeight = FontWeight.ExtraBold,
                            fontSize = 25.sp,
                            color = RosaPrincipal
                        )

                        Text(
                            text = "Descubre nuestros platillos",
                            fontSize = 12.sp,
                            color = TextoSecundario
                        )
                    }
                },

                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = FondoClaro
                )
            )
        }
    ) { innerPadding ->


    Column(

        modifier = Modifier
            .fillMaxSize()
            .padding(innerPadding)
            .verticalScroll(
                rememberScrollState()
            )
    ) {

        Image(

            painter = rememberAsyncImagePainter(
                model = platillo.imagenUrl
                    ?: "https://via.placeholder.com/600"
            ),

            contentDescription =
                platillo.nombre,

            modifier = Modifier
                .fillMaxWidth()
                .height(290.dp)
                .clip(
                    RoundedCornerShape(
                        bottomStart = 30.dp,
                        bottomEnd = 30.dp
                    )
                ),

            contentScale =
                ContentScale.Crop
        )


        Column(

            modifier = Modifier
                .fillMaxWidth()
                .padding(20.dp)
        ) {

            platillo.categoriaNombre?.let {

                Surface(

                    shape =
                        RoundedCornerShape(50.dp),

                    color =
                        VerdeLima
                ) {

                    Text(

                        text = it.uppercase(),

                        modifier = Modifier.padding(
                            horizontal = 13.dp,
                            vertical = 7.dp
                        ),

                        fontSize = 11.sp,

                        fontWeight =
                            FontWeight.ExtraBold
                    )
                }
            }


            Spacer(
                modifier = Modifier.height(10.dp)
            )

            Row(

                modifier =
                    Modifier.fillMaxWidth(),

                verticalAlignment =
                    Alignment.Top
            ) {

                Text(

                    text =
                        platillo.nombre
                            ?: "Sin nombre",

                    modifier =
                        Modifier.weight(1f),

                    fontSize = 28.sp,

                    lineHeight = 32.sp,

                    fontWeight =
                        FontWeight.ExtraBold,

                    color = TextoOscuro
                )


                Spacer(
                    modifier = Modifier.width(10.dp)
                )


                Text(

                    text = String.format(
                        Locale.US,
                        "$%.2f",
                        platillo.precio
                    ),

                    fontSize = 22.sp,

                    fontWeight =
                        FontWeight.ExtraBold,

                    color =
                        RosaPrincipal
                )
            }


            Spacer(
                modifier = Modifier.height(14.dp)
            )

            Row(

                horizontalArrangement =
                    Arrangement.spacedBy(8.dp),

                verticalAlignment =
                    Alignment.CenterVertically
            ) {

                platillo.estado?.let { estado ->

                    val disponible =
                        estado.equals(
                            "Disponible",
                            ignoreCase = true
                        )

                    AssistChip(

                        onClick = {},

                        label = {
                            Text(estado)
                        },

                        colors =
                            AssistChipDefaults.assistChipColors(

                                containerColor =
                                    if (disponible)
                                        VerdeLima.copy(
                                            alpha = 0.35f
                                        )
                                    else
                                        Color(0xFFFFDAD6)
                            )
                    )
                }


                platillo.tiempoPreparacion?.let {

                    AssistChip(

                        onClick = {},

                        label = {
                            Text(
                                "⏱ $it minutos"
                            )
                        }
                    )
                }
            }


            Spacer(
                modifier = Modifier.height(22.dp)
            )


            HorizontalDivider()


            Spacer(
                modifier = Modifier.height(22.dp)
            )

            Text(
                text = "Descripción",
                fontSize = 20.sp,
                fontWeight = FontWeight.ExtraBold
            )

            Spacer(
                modifier = Modifier.height(7.dp)
            )

            Text(

                text = platillo.descripcion
                    ?: "Sin descripción disponible.",

                fontSize = 15.sp,

                lineHeight = 23.sp,

                color = TextoSecundario
            )


            Spacer(
                modifier = Modifier.height(25.dp)
            )

            if (platillo.ingredientes.isNotEmpty()) {

                Text(
                    text = "Ingredientes",
                    fontSize = 20.sp,
                    fontWeight = FontWeight.ExtraBold
                )

                Spacer(
                    modifier = Modifier.height(12.dp)
                )


                Column(
                    verticalArrangement =
                        Arrangement.spacedBy(9.dp)
                ) {

                    platillo.ingredientes.forEach { ingrediente ->

                        Row(
                            verticalAlignment =
                                Alignment.CenterVertically
                        ) {

                            Box(

                                modifier = Modifier
                                    .size(9.dp)
                                    .clip(CircleShape)
                                    .background(
                                        RosaPrincipal
                                    )
                            )

                            Spacer(
                                modifier =
                                    Modifier.width(11.dp)
                            )

                            Text(
                                text = ingrediente,
                                fontSize = 15.sp
                            )
                        }
                    }
                }
            }


            Spacer(
                modifier = Modifier.height(28.dp)
            )

            Text(
                text = "Información del platillo",
                fontSize = 20.sp,
                fontWeight = FontWeight.ExtraBold
            )

            Spacer(
                modifier = Modifier.height(12.dp)
            )


            InfoRow(
                label = "ID del platillo",
                value =
                    platillo.idPlatillo.toString()
            )


            InfoRow(
                label = "ID de categoría",
                value =
                    platillo.idCategoria?.toString()
                        ?: "No disponible"
            )


            InfoRow(
                label = "Categoría",
                value =
                    platillo.categoriaNombre
                        ?: "No disponible"
            )


            InfoRow(
                label = "Estado",
                value =
                    platillo.estado
                        ?: "No disponible"
            )


            InfoRow(
                label = "Tiempo de preparación",
                value =
                    platillo.tiempoPreparacion
                        ?.let { "$it minutos" }
                        ?: "No especificado"
            )


            InfoRow(
                label = "Precio",
                value = String.format(
                    Locale.US,
                    "$%.2f",
                    platillo.precio
                )
            )

            if (!platillo.imagenUrl.isNullOrBlank()) {

                Spacer(
                    modifier = Modifier.height(10.dp)
                )

                Text(
                    text = "Imagen",
                    fontSize = 14.sp,
                    fontWeight = FontWeight.Bold
                )

                Spacer(
                    modifier = Modifier.height(4.dp)
                )

                Text(
                    text = platillo.imagenUrl!!,
                    fontSize = 11.sp,
                    color = TextoSecundario
                )
            }


            Spacer(
                modifier = Modifier.height(30.dp)
            )

            Button(

                onClick = {},

                modifier = Modifier
                    .fillMaxWidth()
                    .height(55.dp),

                shape =
                    RoundedCornerShape(18.dp),

                colors =
                    ButtonDefaults.buttonColors(
                        containerColor =
                            RosaPrincipal
                    )
            ) {

                Text(
                    text = "Ver platillo",
                    fontWeight =
                        FontWeight.Bold,
                    fontSize = 16.sp
                )
            }


            Spacer(
                modifier = Modifier.height(20.dp)
            )
        }
    }
}
}

@Composable
fun InfoRow(
    label: String,
    value: String
) {

    Row(

        modifier = Modifier
            .fillMaxWidth()
            .padding(
                vertical = 7.dp
            ),

        horizontalArrangement =
            Arrangement.SpaceBetween,

        verticalAlignment =
            Alignment.CenterVertically
    ) {

        Text(
            text = label,
            fontSize = 13.sp,
            color = TextoSecundario
        )

        Text(
            text = value,
            modifier =
                Modifier.widthIn(
                    max = 210.dp
                ),
            fontSize = 13.sp,
            fontWeight = FontWeight.Bold
        )
    }
}