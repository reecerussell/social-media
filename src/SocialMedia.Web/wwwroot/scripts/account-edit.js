const InitialiseAccountEdit = () => {
	if (!document.getElementById("accountEdit")) {
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
			btn.classList.remove("btn-primary");
			btn.classList.remove("btn-block");
			btn.classList.add("btn-link");
		} else {
			btn.innerText = btn.getAttribute("data-fallback-text");
			btn.classList.remove("btn-link");
			btn.classList.add("btn-primary");
			btn.classList.add("btn-block");
		}
	});
};

export default InitialiseAccountEdit;
