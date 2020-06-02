const InitialisePostCreate = () => {
	if (!document.getElementById("createPost")) {
		return;
	}

	const btn = document.getElementById("PickFile");
	const file = document.getElementById("File");

	btn.addEventListener("click", (e) => {
		e.preventDefault();
		file.click();
	});

	file.addEventListener("change", (e) => {
		if (e.target.files.length > 0) {
			btn.innerText = e.target.files[0].name;
		} else {
			btn.innerText = btn.getAttribute("data-fallback-text");
		}
	});
};

export default InitialisePostCreate;
