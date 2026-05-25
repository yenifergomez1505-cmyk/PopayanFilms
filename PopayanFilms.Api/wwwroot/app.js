const API = 'http://localhost:5000';

function showSection(name, btn) {
  document.querySelectorAll('.section').forEach(s => s.classList.remove('active'));
  document.querySelectorAll('nav button').forEach(b => b.classList.remove('active'));
  document.getElementById(name).classList.add('active');
  btn.classList.add('active');
  const loaders = { movies: loadMovies, salas: loadSalas, horarios: loadHorarios, listaprecios: loadPrecios, asientos: loadAsientos };
  if (loaders[name]) loaders[name]();
}

function toast(msg, type = 'success') {
  const t = document.getElementById('toast');
  t.textContent = msg;
  t.className = `toast show ${type}`;
  setTimeout(() => t.className = 'toast', 3000);
}

function badge(estado) {
  return `<span class="badge badge-${estado}">${estado}</span>`;
}

// ===================== MOVIES =====================
async function loadMovies() {
  const res = await fetch(`${API}/movies`);
  const data = await res.json();
  const tbody = document.getElementById('movies-tbody');
  if (!data.length) { tbody.innerHTML = '<tr><td colspan="7" class="empty">No hay películas registradas</td></tr>'; return; }
  tbody.innerHTML = data.map(m => `
    <tr>
      <td>${m.id}</td>
      <td><strong>${m.titulo}</strong></td>
      <td>${m.genero}</td>
      <td>${m.duracionMin} min</td>
      <td>${m.clasificacion}</td>
      <td>${badge(m.estado)}</td>
      <td class="actions">
        <button class="btn btn-edit" onclick="editMovie(${m.id})">Editar</button>
        <button class="btn btn-danger" onclick="deleteMovie(${m.id})">Eliminar</button>
      </td>
    </tr>`).join('');
}

async function saveMovie() {
  const id = parseInt(document.getElementById('m-id').value);
  const movie = {
    id,
    titulo: document.getElementById('m-titulo').value,
    genero: document.getElementById('m-genero').value,
    duracionMin: parseInt(document.getElementById('m-duracion').value),
    clasificacion: document.getElementById('m-clasificacion').value,
    fechaEstreno: document.getElementById('m-fecha').value || null,
    estado: document.getElementById('m-estado').value
  };
  const res = await fetch(`${API}/movies${id ? '/'+id : ''}`, {
    method: id ? 'PUT' : 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(movie)
  });
  if (res.ok) { toast(id ? 'Película actualizada ✓' : 'Película creada ✓'); clearMovieForm(); loadMovies(); }
  else { const err = await res.json(); toast(err.error || 'Error', 'error'); }
}

async function editMovie(id) {
  const m = await (await fetch(`${API}/movies/${id}`)).json();
  document.getElementById('m-id').value = m.id;
  document.getElementById('m-titulo').value = m.titulo;
  document.getElementById('m-genero').value = m.genero;
  document.getElementById('m-duracion').value = m.duracionMin;
  document.getElementById('m-clasificacion').value = m.clasificacion;
  document.getElementById('m-fecha').value = m.fechaEstreno || '';
  document.getElementById('m-estado').value = m.estado;
}

async function deleteMovie(id) {
  if (!confirm('¿Eliminar esta película?')) return;
  await fetch(`${API}/movies/${id}`, { method: 'DELETE' });
  toast('Película eliminada'); loadMovies();
}

function clearMovieForm() {
  ['m-titulo','m-genero','m-duracion','m-fecha'].forEach(id => document.getElementById(id).value = '');
  document.getElementById('m-id').value = '0';
  document.getElementById('m-clasificacion').value = '';
  document.getElementById('m-estado').value = 'activa';
}

// ===================== SALAS =====================
async function loadSalas() {
  const data = await (await fetch(`${API}/salas`)).json();
  const tbody = document.getElementById('salas-tbody');
  if (!data.length) { tbody.innerHTML = '<tr><td colspan="5" class="empty">No hay salas registradas</td></tr>'; return; }
  tbody.innerHTML = data.map(s => `
    <tr>
      <td>${s.id}</td>
      <td><strong>${s.nombre}</strong></td>
      <td>${s.capacidad} personas</td>
      <td>${s.tipo}</td>
      <td class="actions">
        <button class="btn btn-edit" onclick="editSala(${s.id})">Editar</button>
        <button class="btn btn-danger" onclick="deleteSala(${s.id})">Eliminar</button>
      </td>
    </tr>`).join('');
}

async function saveSala() {
  const id = parseInt(document.getElementById('s-id').value);
  const sala = { id, nombre: document.getElementById('s-nombre').value, capacidad: parseInt(document.getElementById('s-capacidad').value), tipo: document.getElementById('s-tipo').value };
  const res = await fetch(`${API}/salas${id ? '/'+id : ''}`, { method: id ? 'PUT' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(sala) });
  if (res.ok) { toast(id ? 'Sala actualizada ✓' : 'Sala creada ✓'); clearSalaForm(); loadSalas(); }
  else { const err = await res.json(); toast(err.error || 'Error', 'error'); }
}

async function editSala(id) {
  const s = await (await fetch(`${API}/salas/${id}`)).json();
  document.getElementById('s-id').value = s.id;
  document.getElementById('s-nombre').value = s.nombre;
  document.getElementById('s-capacidad').value = s.capacidad;
  document.getElementById('s-tipo').value = s.tipo;
}

async function deleteSala(id) {
  if (!confirm('¿Eliminar esta sala?')) return;
  await fetch(`${API}/salas/${id}`, { method: 'DELETE' });
  toast('Sala eliminada'); loadSalas();
}

function clearSalaForm() {
  ['s-nombre','s-capacidad'].forEach(id => document.getElementById(id).value = '');
  document.getElementById('s-id').value = '0';
  document.getElementById('s-tipo').value = '';
}

// ===================== HORARIOS =====================
async function loadHorarios() {
  const data = await (await fetch(`${API}/horarios`)).json();
  const tbody = document.getElementById('horarios-tbody');
  if (!data.length) { tbody.innerHTML = '<tr><td colspan="6" class="empty">No hay horarios registrados</td></tr>'; return; }
  tbody.innerHTML = data.map(h => `
    <tr>
      <td>${h.id}</td>
      <td>${h.fechaHora}</td>
      <td>${h.idSala}</td>
      <td>${h.idPelicula}</td>
      <td>${badge(h.estado)}</td>
      <td class="actions">
        <button class="btn btn-edit" onclick="editHorario(${h.id})">Editar</button>
        <button class="btn btn-danger" onclick="deleteHorario(${h.id})">Eliminar</button>
      </td>
    </tr>`).join('');
}

async function saveHorario() {
  const id = parseInt(document.getElementById('h-id').value);
  const horario = { id, fechaHora: document.getElementById('h-fechahora').value, idSala: parseInt(document.getElementById('h-idsala').value), idPelicula: parseInt(document.getElementById('h-idpelicula').value), estado: document.getElementById('h-estado').value };
  const res = await fetch(`${API}/horarios${id ? '/'+id : ''}`, { method: id ? 'PUT' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(horario) });
  if (res.ok) { toast(id ? 'Horario actualizado ✓' : 'Horario creado ✓'); clearHorarioForm(); loadHorarios(); }
  else { const err = await res.json(); toast(err.error || 'Error', 'error'); }
}

async function editHorario(id) {
  const h = await (await fetch(`${API}/horarios/${id}`)).json();
  document.getElementById('h-id').value = h.id;
  document.getElementById('h-fechahora').value = h.fechaHora;
  document.getElementById('h-idsala').value = h.idSala;
  document.getElementById('h-idpelicula').value = h.idPelicula;
  document.getElementById('h-estado').value = h.estado;
}

async function deleteHorario(id) {
  if (!confirm('¿Eliminar este horario?')) return;
  await fetch(`${API}/horarios/${id}`, { method: 'DELETE' });
  toast('Horario eliminado'); loadHorarios();
}

function clearHorarioForm() {
  ['h-fechahora','h-idsala','h-idpelicula'].forEach(id => document.getElementById(id).value = '');
  document.getElementById('h-id').value = '0';
  document.getElementById('h-estado').value = 'activo';
}

// ===================== LISTA PRECIOS =====================
async function loadPrecios() {
  const data = await (await fetch(`${API}/listaprecios`)).json();
  const tbody = document.getElementById('precios-tbody');
  if (!data.length) { tbody.innerHTML = '<tr><td colspan="5" class="empty">No hay precios registrados</td></tr>'; return; }
  tbody.innerHTML = data.map(p => `
    <tr>
      <td>${p.id}</td>
      <td>${p.descripcion}</td>
      <td>$${p.precio.toLocaleString()}</td>
      <td>${p.tipo}</td>
      <td class="actions">
        <button class="btn btn-edit" onclick="editPrecio(${p.id})">Editar</button>
        <button class="btn btn-danger" onclick="deletePrecio(${p.id})">Eliminar</button>
      </td>
    </tr>`).join('');
}

async function savePrecio() {
  const id = parseInt(document.getElementById('p-id').value);
  const precio = { id, descripcion: document.getElementById('p-descripcion').value, precio: parseFloat(document.getElementById('p-precio').value), tipo: document.getElementById('p-tipo').value };
  const res = await fetch(`${API}/listaprecios${id ? '/'+id : ''}`, { method: id ? 'PUT' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(precio) });
  if (res.ok) { toast(id ? 'Precio actualizado ✓' : 'Precio creado ✓'); clearPrecioForm(); loadPrecios(); }
  else { const err = await res.json(); toast(err.error || 'Error', 'error'); }
}

async function editPrecio(id) {
  const p = await (await fetch(`${API}/listaprecios/${id}`)).json();
  document.getElementById('p-id').value = p.id;
  document.getElementById('p-descripcion').value = p.descripcion;
  document.getElementById('p-precio').value = p.precio;
  document.getElementById('p-tipo').value = p.tipo;
}

async function deletePrecio(id) {
  if (!confirm('¿Eliminar este precio?')) return;
  await fetch(`${API}/listaprecios/${id}`, { method: 'DELETE' });
  toast('Precio eliminado'); loadPrecios();
}

function clearPrecioForm() {
  ['p-descripcion','p-precio'].forEach(id => document.getElementById(id).value = '');
  document.getElementById('p-id').value = '0';
  document.getElementById('p-tipo').value = '';
}

// ===================== ASIENTOS =====================
async function loadAsientos() {
  const data = await (await fetch(`${API}/asientos`)).json();
  const tbody = document.getElementById('asientos-tbody');
  if (!data.length) { tbody.innerHTML = '<tr><td colspan="6" class="empty">No hay asientos registrados</td></tr>'; return; }
  tbody.innerHTML = data.map(a => `
    <tr>
      <td>${a.id}</td>
      <td>${a.numero}</td>
      <td>${a.fila}</td>
      <td>${a.idSala}</td>
      <td>${badge(a.estado)}</td>
      <td class="actions">
        <button class="btn btn-edit" onclick="editAsiento(${a.id})">Editar</button>
        <button class="btn btn-danger" onclick="deleteAsiento(${a.id})">Eliminar</button>
      </td>
    </tr>`).join('');
}

async function saveAsiento() {
  const id = parseInt(document.getElementById('a-id').value);
  const asiento = { id, numero: parseInt(document.getElementById('a-numero').value), fila: document.getElementById('a-fila').value, idSala: parseInt(document.getElementById('a-idsala').value), estado: document.getElementById('a-estado').value };
  const res = await fetch(`${API}/asientos${id ? '/'+id : ''}`, { method: id ? 'PUT' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(asiento) });
  if (res.ok) { toast(id ? 'Asiento actualizado ✓' : 'Asiento creado ✓'); clearAsientoForm(); loadAsientos(); }
  else { const err = await res.json(); toast(err.error || 'Error', 'error'); }
}

async function editAsiento(id) {
  const a = await (await fetch(`${API}/asientos/${id}`)).json();
  document.getElementById('a-id').value = a.id;
  document.getElementById('a-numero').value = a.numero;
  document.getElementById('a-fila').value = a.fila;
  document.getElementById('a-idsala').value = a.idSala;
  document.getElementById('a-estado').value = a.estado;
}

async function deleteAsiento(id) {
  if (!confirm('¿Eliminar este asiento?')) return;
  await fetch(`${API}/asientos/${id}`, { method: 'DELETE' });
  toast('Asiento eliminado'); loadAsientos();
}

function clearAsientoForm() {
  ['a-numero','a-fila','a-idsala'].forEach(id => document.getElementById(id).value = '');
  document.getElementById('a-id').value = '0';
  document.getElementById('a-estado').value = 'disponible';
}

// Cargar películas al inicio
loadMovies();