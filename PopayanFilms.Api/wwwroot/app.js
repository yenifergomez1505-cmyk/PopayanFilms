const API = 'http://localhost:5000';

const sectionTitles = {
  movies: 'Películas', salas: 'Salas', horarios: 'Horarios',
  asientos: 'Asientos', listaprecios: 'Lista de Precios',
  reservas: 'Reservas', tickets: 'Tickets'
};

function showSection(name, el) {
  document.querySelectorAll('.section').forEach(s => s.classList.remove('active'));
  document.querySelectorAll('.nav-item').forEach(n => n.classList.remove('active'));
  document.getElementById(name).classList.add('active');
  el.classList.add('active');
  document.getElementById('topbar-title').textContent = sectionTitles[name];
  const loaders = {
    movies: loadMovies, salas: loadSalas, horarios: loadHorarios,
    asientos: loadAsientos, listaprecios: loadPrecios,
    reservas: loadReservas, tickets: loadTickets
  };
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
  const data = await fetch(`${API}/movies`).then(r => r.json());
  document.getElementById('movies-count').textContent = data.length;
  const tbody = document.getElementById('movies-tbody');
  if (!data.length) { tbody.innerHTML = '<tr><td colspan="7" class="empty">No hay películas registradas</td></tr>'; return; }
  tbody.innerHTML = data.map(m => `
    <tr>
      <td style="color:var(--muted)">${m.id}</td>
      <td><strong>${m.titulo}</strong></td>
      <td style="color:var(--muted)">${m.genero}</td>
      <td>${m.duracionMin} min</td>
      <td>${m.clasificacion}</td>
      <td>${badge(m.estado)}</td>
      <td class="actions">
        <button class="btn btn-edit" onclick="editMovie(${m.id})">✎ Editar</button>
        <button class="btn btn-danger" onclick="deleteMovie(${m.id})">✕</button>
      </td>
    </tr>`).join('');
}

async function saveMovie() {
  const id = parseInt(document.getElementById('m-id').value);
  const movie = {
    id, titulo: document.getElementById('m-titulo').value,
    genero: document.getElementById('m-genero').value,
    duracionMin: parseInt(document.getElementById('m-duracion').value),
    clasificacion: document.getElementById('m-clasificacion').value,
    fechaEstreno: document.getElementById('m-fecha').value || null,
    estado: document.getElementById('m-estado').value
  };
  const res = await fetch(`${API}/movies${id ? '/'+id : ''}`, {
    method: id ? 'PUT' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(movie)
  });
  if (res.ok) { toast(id ? '✓ Película actualizada' : '✓ Película creada'); clearMovieForm(); loadMovies(); }
  else { const err = await res.json(); toast(err.error || 'Error', 'error'); }
}

async function editMovie(id) {
  const m = await fetch(`${API}/movies/${id}`).then(r => r.json());
  document.getElementById('m-id').value = m.id;
  document.getElementById('m-titulo').value = m.titulo;
  document.getElementById('m-genero').value = m.genero;
  document.getElementById('m-duracion').value = m.duracionMin;
  document.getElementById('m-clasificacion').value = m.clasificacion;
  document.getElementById('m-fecha').value = m.fechaEstreno || '';
  document.getElementById('m-estado').value = m.estado;
  document.getElementById('movie-form-title').textContent = 'Editar Película';
}

async function deleteMovie(id) {
  if (!confirm('¿Eliminar esta película?')) return;
  await fetch(`${API}/movies/${id}`, { method: 'DELETE' });
  toast('Película eliminada'); loadMovies();
}

function clearMovieForm() {
  ['m-titulo','m-genero','m-duracion','m-fecha'].forEach(i => document.getElementById(i).value = '');
  document.getElementById('m-id').value = '0';
  document.getElementById('m-clasificacion').value = '';
  document.getElementById('m-estado').value = 'activa';
  document.getElementById('movie-form-title').textContent = 'Nueva Película';
}

// ===================== SALAS =====================
async function loadSalas() {
  const data = await fetch(`${API}/salas`).then(r => r.json());
  document.getElementById('salas-count').textContent = data.length;
  const tbody = document.getElementById('salas-tbody');
  if (!data.length) { tbody.innerHTML = '<tr><td colspan="5" class="empty">No hay salas registradas</td></tr>'; return; }
  tbody.innerHTML = data.map(s => `
    <tr>
      <td style="color:var(--muted)">${s.id}</td>
      <td><strong>${s.nombre}</strong></td>
      <td>${s.capacidad} personas</td>
      <td><span class="badge badge-activo">${s.tipo}</span></td>
      <td class="actions">
        <button class="btn btn-edit" onclick="editSala(${s.id})">✎ Editar</button>
        <button class="btn btn-danger" onclick="deleteSala(${s.id})">✕</button>
      </td>
    </tr>`).join('');
}

async function saveSala() {
  const id = parseInt(document.getElementById('s-id').value);
  const sala = { id, nombre: document.getElementById('s-nombre').value, capacidad: parseInt(document.getElementById('s-capacidad').value), tipo: document.getElementById('s-tipo').value };
  const res = await fetch(`${API}/salas${id ? '/'+id : ''}`, { method: id ? 'PUT' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(sala) });
  if (res.ok) { toast(id ? '✓ Sala actualizada' : '✓ Sala creada'); clearSalaForm(); loadSalas(); }
  else { const err = await res.json(); toast(err.error || 'Error', 'error'); }
}

async function editSala(id) {
  const s = await fetch(`${API}/salas/${id}`).then(r => r.json());
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
  ['s-nombre','s-capacidad'].forEach(i => document.getElementById(i).value = '');
  document.getElementById('s-id').value = '0';
  document.getElementById('s-tipo').value = '';
}

// ===================== HORARIOS =====================
async function loadHorarios() {
  const data = await fetch(`${API}/horarios`).then(r => r.json());
  document.getElementById('horarios-count').textContent = data.length;
  const tbody = document.getElementById('horarios-tbody');
  if (!data.length) { tbody.innerHTML = '<tr><td colspan="6" class="empty">No hay horarios registrados</td></tr>'; return; }
  tbody.innerHTML = data.map(h => `
    <tr>
      <td style="color:var(--muted)">${h.id}</td>
      <td>${h.fechaHora}</td>
      <td>${h.idSala}</td>
      <td>${h.idPelicula}</td>
      <td>${badge(h.estado)}</td>
      <td class="actions">
        <button class="btn btn-edit" onclick="editHorario(${h.id})">✎ Editar</button>
        <button class="btn btn-danger" onclick="deleteHorario(${h.id})">✕</button>
      </td>
    </tr>`).join('');
}

async function saveHorario() {
  const id = parseInt(document.getElementById('h-id').value);
  const horario = { id, fechaHora: document.getElementById('h-fechahora').value, idSala: parseInt(document.getElementById('h-idsala').value), idPelicula: parseInt(document.getElementById('h-idpelicula').value), estado: document.getElementById('h-estado').value };
  const res = await fetch(`${API}/horarios${id ? '/'+id : ''}`, { method: id ? 'PUT' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(horario) });
  if (res.ok) { toast(id ? '✓ Horario actualizado' : '✓ Horario creado'); clearHorarioForm(); loadHorarios(); }
  else { const err = await res.json(); toast(err.error || 'Error', 'error'); }
}

async function editHorario(id) {
  const h = await fetch(`${API}/horarios/${id}`).then(r => r.json());
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
  ['h-fechahora','h-idsala','h-idpelicula'].forEach(i => document.getElementById(i).value = '');
  document.getElementById('h-id').value = '0';
  document.getElementById('h-estado').value = 'activo';
}

// ===================== ASIENTOS =====================
async function loadAsientos() {
  const data = await fetch(`${API}/asientos`).then(r => r.json());
  document.getElementById('asientos-count').textContent = data.length;
  const tbody = document.getElementById('asientos-tbody');
  if (!data.length) { tbody.innerHTML = '<tr><td colspan="6" class="empty">No hay asientos registrados</td></tr>'; return; }
  tbody.innerHTML = data.map(a => `
    <tr>
      <td style="color:var(--muted)">${a.id}</td>
      <td>${a.numero}</td>
      <td><strong>${a.fila}</strong></td>
      <td>${a.idSala}</td>
      <td>${badge(a.estado)}</td>
      <td class="actions">
        <button class="btn btn-edit" onclick="editAsiento(${a.id})">✎ Editar</button>
        <button class="btn btn-danger" onclick="deleteAsiento(${a.id})">✕</button>
      </td>
    </tr>`).join('');
}

async function saveAsiento() {
  const id = parseInt(document.getElementById('a-id').value);
  const asiento = { id, numero: parseInt(document.getElementById('a-numero').value), fila: document.getElementById('a-fila').value, idSala: parseInt(document.getElementById('a-idsala').value), estado: document.getElementById('a-estado').value };
  const res = await fetch(`${API}/asientos${id ? '/'+id : ''}`, { method: id ? 'PUT' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(asiento) });
  if (res.ok) { toast(id ? '✓ Asiento actualizado' : '✓ Asiento creado'); clearAsientoForm(); loadAsientos(); }
  else { const err = await res.json(); toast(err.error || 'Error', 'error'); }
}

async function editAsiento(id) {
  const a = await fetch(`${API}/asientos/${id}`).then(r => r.json());
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
  ['a-numero','a-fila','a-idsala'].forEach(i => document.getElementById(i).value = '');
  document.getElementById('a-id').value = '0';
  document.getElementById('a-estado').value = 'disponible';
}

// ===================== LISTA PRECIOS =====================
async function loadPrecios() {
  const data = await fetch(`${API}/listaprecios`).then(r => r.json());
  document.getElementById('precios-count').textContent = data.length;
  const tbody = document.getElementById('precios-tbody');
  if (!data.length) { tbody.innerHTML = '<tr><td colspan="5" class="empty">No hay precios registrados</td></tr>'; return; }
  tbody.innerHTML = data.map(p => `
    <tr>
      <td style="color:var(--muted)">${p.id}</td>
      <td><strong>${p.descripcion}</strong></td>
      <td style="color:var(--gold)">$${p.precio.toLocaleString()}</td>
      <td>${p.tipo}</td>
      <td class="actions">
        <button class="btn btn-edit" onclick="editPrecio(${p.id})">✎ Editar</button>
        <button class="btn btn-danger" onclick="deletePrecio(${p.id})">✕</button>
      </td>
    </tr>`).join('');
}

async function savePrecio() {
  const id = parseInt(document.getElementById('p-id').value);
  const precio = { id, descripcion: document.getElementById('p-descripcion').value, precio: parseFloat(document.getElementById('p-precio').value), tipo: document.getElementById('p-tipo').value };
  const res = await fetch(`${API}/listaprecios${id ? '/'+id : ''}`, { method: id ? 'PUT' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(precio) });
  if (res.ok) { toast(id ? '✓ Precio actualizado' : '✓ Precio creado'); clearPrecioForm(); loadPrecios(); }
  else { const err = await res.json(); toast(err.error || 'Error', 'error'); }
}

async function editPrecio(id) {
  const p = await fetch(`${API}/listaprecios/${id}`).then(r => r.json());
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
  ['p-descripcion','p-precio'].forEach(i => document.getElementById(i).value = '');
  document.getElementById('p-id').value = '0';
  document.getElementById('p-tipo').value = '';
}

// ===================== RESERVAS =====================
async function loadReservas() {
  const data = await fetch(`${API}/reservas`).then(r => r.json());
  document.getElementById('reservas-count').textContent = data.length;
  const tbody = document.getElementById('reservas-tbody');
  if (!data.length) { tbody.innerHTML = '<tr><td colspan="7" class="empty">No hay reservas registradas</td></tr>'; return; }
  tbody.innerHTML = data.map(r => `
    <tr>
      <td style="color:var(--muted)">${r.id}</td>
      <td>${r.idHorario}</td>
      <td>${r.idAsiento}</td>
      <td>${r.idListaPrecio}</td>
      <td style="color:var(--muted)">${r.fechaReserva}</td>
      <td>${badge(r.estado)}</td>
      <td class="actions">
        <button class="btn btn-edit" onclick="editReserva(${r.id})">✎ Editar</button>
        <button class="btn btn-danger" onclick="deleteReserva(${r.id})">✕</button>
      </td>
    </tr>`).join('');
}

async function saveReserva() {
  const id = parseInt(document.getElementById('r-id').value);
  const reserva = { id, idHorario: parseInt(document.getElementById('r-idhorario').value), idAsiento: parseInt(document.getElementById('r-idasiento').value), idListaPrecio: parseInt(document.getElementById('r-idlistaprecio').value), fechaReserva: '', estado: document.getElementById('r-estado').value };
  const res = await fetch(`${API}/reservas${id ? '/'+id : ''}`, { method: id ? 'PUT' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(reserva) });
  if (res.ok) { toast(id ? '✓ Reserva actualizada' : '✓ Reserva creada'); clearReservaForm(); loadReservas(); }
  else { const err = await res.json(); toast(err.error || 'Error', 'error'); }
}

async function editReserva(id) {
  const r = await fetch(`${API}/reservas/${id}`).then(r => r.json());
  document.getElementById('r-id').value = r.id;
  document.getElementById('r-idhorario').value = r.idHorario;
  document.getElementById('r-idasiento').value = r.idAsiento;
  document.getElementById('r-idlistaprecio').value = r.idListaPrecio;
  document.getElementById('r-estado').value = r.estado;
}

async function deleteReserva(id) {
  if (!confirm('¿Eliminar esta reserva?')) return;
  await fetch(`${API}/reservas/${id}`, { method: 'DELETE' });
  toast('Reserva eliminada'); loadReservas();
}

function clearReservaForm() {
  ['r-idhorario','r-idasiento','r-idlistaprecio'].forEach(i => document.getElementById(i).value = '');
  document.getElementById('r-id').value = '0';
  document.getElementById('r-estado').value = 'pendiente';
}

// ===================== TICKETS =====================
async function loadTickets() {
  const data = await fetch(`${API}/tickets`).then(r => r.json());
  document.getElementById('tickets-count').textContent = data.length;
  const tbody = document.getElementById('tickets-tbody');
  if (!data.length) { tbody.innerHTML = '<tr><td colspan="6" class="empty">No hay tickets emitidos</td></tr>'; return; }
  tbody.innerHTML = data.map(t => `
    <tr>
      <td style="color:var(--muted)">${t.id}</td>
      <td style="font-family:monospace;color:var(--gold)">${t.codigo}</td>
      <td>${t.idReserva}</td>
      <td style="color:var(--muted)">${t.fechaEmision}</td>
      <td>${badge(t.estado)}</td>
      <td class="actions">
        <button class="btn btn-edit" onclick="editTicket(${t.id})">✎ Editar</button>
        <button class="btn btn-danger" onclick="deleteTicket(${t.id})">✕</button>
      </td>
    </tr>`).join('');
}

async function saveTicket() {
  const id = parseInt(document.getElementById('t-id').value);
  const ticket = { id, idReserva: parseInt(document.getElementById('t-idreserva').value), codigo: '', fechaEmision: '', estado: document.getElementById('t-estado').value };
  const res = await fetch(`${API}/tickets${id ? '/'+id : ''}`, { method: id ? 'PUT' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(ticket) });
  if (res.ok) { toast(id ? '✓ Ticket actualizado' : '✓ Ticket emitido'); clearTicketForm(); loadTickets(); }
  else { const err = await res.json(); toast(err.error || 'Error', 'error'); }
}

async function editTicket(id) {
  const t = await fetch(`${API}/tickets/${id}`).then(r => r.json());
  document.getElementById('t-id').value = t.id;
  document.getElementById('t-idreserva').value = t.idReserva;
  document.getElementById('t-estado').value = t.estado;
}

async function deleteTicket(id) {
  if (!confirm('¿Eliminar este ticket?')) return;
  await fetch(`${API}/tickets/${id}`, { method: 'DELETE' });
  toast('Ticket eliminado'); loadTickets();
}

function clearTicketForm() {
  document.getElementById('t-idreserva').value = '';
  document.getElementById('t-id').value = '0';
  document.getElementById('t-estado').value = 'activo';
}

// Cargar al inicio
loadMovies();