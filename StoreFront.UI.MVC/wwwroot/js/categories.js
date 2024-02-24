const successStyle = 'alert alert-success text-center'
const failStyle = 'alert alert-danger text-center'

function deleteConfirmed(response) {
    let rowId = '#category-' + response.id
    console.log(rowId)
    $('#categoriesTable').find(rowId).remove()
    $('#messageContent').removeClass().addClass(successStyle).text(response.message)
}
function deleteFailed() {
    $('#messageContent').removeClass().addClass(failStyle).text('Delete unsuccessful')
}