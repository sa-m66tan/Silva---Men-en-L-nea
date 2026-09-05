package sv.org.arrupe.retrofitapi.viewmodel

import android.util.Log
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import sv.org.arrupe.retrofitapi.data.SilvaServiceFactory
import sv.org.arrupe.retrofitapi.model.Platillo

class PlatilloViewModel : ViewModel() {
    private val silvaService = SilvaServiceFactory.makeSilvaService()

    private val _platillosList = MutableStateFlow<List<Platillo>>(emptyList())
    val platillosList: StateFlow<List<Platillo>> = _platillosList

    private val _errorMessage = MutableStateFlow<String?>(null)
    val errorMessage: StateFlow<String?> = _errorMessage

    init {
        fetchPlatillos()
    }

    fun fetchPlatillos() {
        viewModelScope.launch {
            try {
                _errorMessage.value = null
                val response = silvaService.listPlatillos()
                _platillosList.value = response
            } catch (e: Exception) {
                Log.e("PlatilloViewModel", "Error al cargar platillos", e)
                _errorMessage.value = "Error de conexión: ${e.localizedMessage}"
                _platillosList.value = emptyList()
            }
        }
    }
}