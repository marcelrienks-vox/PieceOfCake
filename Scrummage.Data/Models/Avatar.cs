﻿
namespace Scrummage.Data.Models {
	public class Avatar {

		#region Properties

		public int Id { get; set; }
		public byte[] Image { get; set; }
		
        #endregion

		#region Navigation
        
		public virtual Member Member { get; set; }
		
        #endregion
	}
}